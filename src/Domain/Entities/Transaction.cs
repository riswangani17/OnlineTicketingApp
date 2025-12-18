namespace OnlineTicketingApp.Domain.Entities;

public class Transaction : BaseAuditableEntity
{
    public int EventId { get; set; }
    public Event? Event { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime TransactionDate { get; set; }
    public TransactionStatus Status { get; set; }
    public string TicketCode { get; set; } = string.Empty;
}