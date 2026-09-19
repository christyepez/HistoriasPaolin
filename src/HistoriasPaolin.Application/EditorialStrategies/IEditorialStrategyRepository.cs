using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Integration;

namespace HistoriasPaolin.Application.EditorialStrategies;

public interface IEditorialStrategyRepository
{
    Task<Channel?> GetChannelAsync(Guid channelId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EditorialStrategy>> ListByChannelAsync(Guid channelId, CancellationToken cancellationToken);
    Task<EditorialStrategy?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<EditorialStrategy?> GetActiveAsync(Guid channelId, DateTime atUtc, CancellationToken cancellationToken);
    Task AddAsync(EditorialStrategy strategy, CancellationToken cancellationToken);
    void AddOutboxMessage(OutboxMessage message);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void SetOriginalRowVersion(EditorialStrategy strategy, byte[] rowVersion);
}
