using OnlineTicketingApp.Domain.Entities;

namespace OnlineTicketingApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<Event> Events { get; }

    DbSet<Customer> Customers { get; }
    
    DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
