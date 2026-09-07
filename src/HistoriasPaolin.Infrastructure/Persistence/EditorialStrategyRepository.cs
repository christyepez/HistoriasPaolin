using HistoriasPaolin.Application.EditorialStrategies;
using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Integration;
using Microsoft.EntityFrameworkCore;

namespace HistoriasPaolin.Infrastructure.Persistence;

public sealed class EditorialStrategyRepository(HistoriasPaolinDbContext dbContext) : IEditorialStrategyRepository
{
    public Task<Channel?> GetChannelAsync(Guid channelId, CancellationToken cancellationToken) =>
        dbContext.Channels
            .Include(channel => channel.EditorialStrategies)
            .SingleOrDefaultAsync(channel => channel.Id == channelId, cancellationToken);

    public async Task<IReadOnlyList<EditorialStrategy>> ListByChannelAsync(Guid channelId, CancellationToken cancellationToken) =>
        await dbContext.EditorialStrategies
            .AsNoTracking()
            .Where(strategy => strategy.ChannelId == channelId)
            .OrderByDescending(strategy => strategy.IsActive)
            .ThenByDescending(strategy => strategy.EffectiveFromUtc)
            .ToListAsync(cancellationToken);

    public Task<EditorialStrategy?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.EditorialStrategies
            .Include(strategy => strategy.Channel)
                .ThenInclude(channel => channel!.EditorialStrategies)
            .Include(strategy => strategy.Pillars)
            .Include(strategy => strategy.Topics)
            .Include(strategy => strategy.Restrictions)
            .SingleOrDefaultAsync(strategy => strategy.Id == id, cancellationToken);

    public Task<EditorialStrategy?> GetActiveAsync(Guid channelId, DateTime atUtc, CancellationToken cancellationToken) =>
        dbContext.EditorialStrategies
            .AsNoTracking()
            .Where(strategy => strategy.ChannelId == channelId && strategy.IsActive && strategy.EffectiveFromUtc <= atUtc && (!strategy.EffectiveToUtc.HasValue || strategy.EffectiveToUtc > atUtc))
            .OrderByDescending(strategy => strategy.EffectiveFromUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(EditorialStrategy strategy, CancellationToken cancellationToken) =>
        await dbContext.EditorialStrategies.AddAsync(strategy, cancellationToken);

    public void AddOutboxMessage(OutboxMessage message) => dbContext.OutboxMessages.Add(message);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EditorialStrategyConcurrencyException();
        }
    }

    public void SetOriginalRowVersion(EditorialStrategy strategy, byte[] rowVersion) =>
        dbContext.Entry(strategy).Property(x => x.RowVersion).OriginalValue = rowVersion;
}
