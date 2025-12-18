using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OnlineTicketingApp.Infrastructure.Identity;

namespace OnlineTicketingApp.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        // base route: /api/Auth
        // endpoint:
        // POST /api/Auth/login
        // POST /api/Auth/register
        // GET  /api/Auth/me
        group.MapPost(Login, "login").AllowAnonymous();
        group.MapPost(Register, "register").AllowAnonymous();
        group.MapGet(Me, "me").RequireAuthorization();
    }

    public record LoginRequest(string UserNameOrEmail, string Password);
    public record RegisterRequest(string Email, string Password);
    public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);

    public async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> Login(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        LoginRequest req)
    {
        var user =
            await userManager.FindByNameAsync(req.UserNameOrEmail)
            ?? await userManager.FindByEmailAsync(req.UserNameOrEmail);

        if (user is null)
            return TypedResults.Unauthorized();

        var ok = await userManager.CheckPasswordAsync(user, req.Password);
        if (!ok)
            return TypedResults.Unauthorized();

        var (token, expiresAtUtc) = CreateJwt(config, user);

        return TypedResults.Ok(new LoginResponse(token, expiresAtUtc));
    }

    public async Task<Results<Ok, BadRequest<object>>> Register(
        UserManager<ApplicationUser> userManager,
        RegisterRequest req)
    {
        var user = new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email
        };

        var result = await userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return TypedResults.BadRequest((object)new { errors });
        }

        return TypedResults.Ok();
    }

    public Ok<object> Me(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var name =
            user.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
            ?? user.Identity?.Name;

        return TypedResults.Ok((object)new { sub, name });
    }

    private static (string Token, DateTime ExpiresAtUtc) CreateJwt(
        IConfiguration config,
        ApplicationUser user)
    {
        var jwt = config.GetSection("Jwt");
        var key = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key not set");
        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];
        var minutes = int.TryParse(jwt["ExpiresMinutes"], out var m) ? m : 120;

        var expiresAt = DateTime.UtcNow.AddMinutes(minutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwtString, expiresAt);
    }
}
