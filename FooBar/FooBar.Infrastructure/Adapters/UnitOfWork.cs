using FooBar.Application.Ports;
using FooBar.Infrastructure.DataSource;
using Microsoft.EntityFrameworkCore;

namespace FooBar.Infrastructure.Adapters;

public class UnitOfWork(DataContext context) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken? cancellationToken = null)
    {
        var token = cancellationToken ?? new CancellationTokenSource().Token;

        context.ChangeTracker.DetectChanges();

        var entryStatus = new Dictionary<EntityState, string> {
            {EntityState.Added, "CreatedOn"},
            {EntityState.Modified, "LastModifiedOn"}
        };

        foreach (var entry in context.ChangeTracker.Entries().Where(entity => entryStatus.ContainsKey(entity.State)))
        {
            entry.Property(entryStatus[entry.State]).CurrentValue = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(token);
    }
}
