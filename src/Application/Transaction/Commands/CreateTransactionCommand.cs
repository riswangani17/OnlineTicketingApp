using MediatR;
using Microsoft.EntityFrameworkCore;
using OnlineTicketingApp.Application.Common.Interfaces;
using OnlineTicketingApp.Domain.Entities;
using OnlineTicketingApp.Domain.Enums;

public record CreateTransactionCommand : IRequest<int>
{
    public int EventId { get; init; }
    public int Quantity { get; init; }

    // Data Customer (Input dari form React)
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;
    public string? CustomerEmail { get; init; }
}

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTransactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var eventEntity = await _context.Events.FindAsync([request.EventId], cancellationToken);
        if (eventEntity == null) throw new Exception("Event not found");
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.PhoneNumber == request.CustomerPhone, cancellationToken);

        if (customer == null)
        {
            customer = new Customer
            {
                Name = request.CustomerName,
                PhoneNumber = request.CustomerPhone,
                Email = request.CustomerEmail,
                TotalSpent = 0,
                LastVisitDate = DateTime.UtcNow
            };
            _context.Customers.Add(customer);
        }
        else
        {
            customer.Name = request.CustomerName;
            if (!string.IsNullOrEmpty(request.CustomerEmail)) customer.Email = request.CustomerEmail;
            customer.LastVisitDate = DateTime.UtcNow;
        }

        var totalAmount = eventEntity.Price * request.Quantity;
        customer.TotalSpent += totalAmount;
        var transaction = new Transaction
        {
            EventId = request.EventId,
            Customer = customer,
            Quantity = request.Quantity,
            TotalAmount = totalAmount,
            Status = TransactionStatus.Pending,
            TicketCode = $"TKT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}",
            TransactionDate = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}