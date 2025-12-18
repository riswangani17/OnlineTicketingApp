namespace OnlineTicketingApp.Domain.Entities;

public class Event : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public decimal Price { get; set; }
    public int AvailableTickets { get; set; }
}