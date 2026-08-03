using HistoriasPaolin.Application.Common;
using HistoriasPaolin.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HistoriasPaolin.Infrastructure.Persistence;

public sealed class AuditingSaveChangesInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = DateTime.UtcNow;
        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.CreatedBy = currentUser.UserName;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
                entry.Entity.UpdatedBy = currentUser.UserName;
            }
        }
    }
}
