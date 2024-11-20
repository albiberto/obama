using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Obama.Domain;

namespace Obama.Infrastructure.Interceptors;

public class AuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SaveChanges(eventData);

        return ValueTask.FromResult(result);
    }

    private static void SaveChanges(DbContextEventData eventData)
    {
        var context = eventData.Context!;

        SaveChanges(context);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context!;

        SaveChanges(context);

        return result;
    }

    private static void SaveChanges(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<Auditable>())
            switch (entry)
            {
                case { State: EntityState.Added }:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case { State: EntityState.Modified }:
                    entry.Property("CreatedAt").IsModified = false;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
    }
}