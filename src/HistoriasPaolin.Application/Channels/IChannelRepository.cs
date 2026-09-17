using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Integration;

namespace HistoriasPaolin.Application.Channels;

public interface IChannelRepository
{
    Task<IReadOnlyList<Channel>> ListAsync(CancellationToken cancellationToken);
    Task<Channel?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Channel?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, Guid? exceptId, CancellationToken cancellationToken);
    Task AddAsync(Channel channel, CancellationToken cancellationToken);
    void AddOutboxMessage(OutboxMessage message);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void SetOriginalRowVersion(Channel channel, byte[] rowVersion);
    void SetOriginalRowVersion(ChannelBrand brand, byte[] rowVersion);
}
