namespace OnlineTicketingApp.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public decimal TotalSpent { get; set; }
    public bool IsSubscribedToPromo { get; set; } = true;
}
