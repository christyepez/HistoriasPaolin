using HistoriasPaolin.Application.Channels;
using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Integration;
using Microsoft.EntityFrameworkCore;

namespace HistoriasPaolin.Infrastructure.Persistence;

public sealed class ChannelRepository(HistoriasPaolinDbContext dbContext) : IChannelRepository
{
    public async Task<IReadOnlyList<Channel>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Channels
            .AsNoTracking()
            .OrderBy(channel => channel.Name)
            .ToListAsync(cancellationToken);

    public Task<Channel?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Channels
            .Include(channel => channel.Brands)
            .SingleOrDefaultAsync(channel => channel.Id == id, cancellationToken);

    public Task<Channel?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
        dbContext.Channels
            .Include(channel => channel.Brands)
            .SingleOrDefaultAsync(channel => channel.Code == code, cancellationToken);

    public Task<bool> CodeExistsAsync(string code, Guid? exceptId, CancellationToken cancellationToken) =>
        dbContext.Channels.AnyAsync(channel => channel.Code == code && (!exceptId.HasValue || channel.Id != exceptId.Value), cancellationToken);

    public async Task AddAsync(Channel channel, CancellationToken cancellationToken) =>
        await dbContext.Channels.AddAsync(channel, cancellationToken);

    public void AddOutboxMessage(OutboxMessage message) => dbContext.OutboxMessages.Add(message);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ChannelConcurrencyException();
        }
    }

    public void SetOriginalRowVersion(Channel channel, byte[] rowVersion) =>
        dbContext.Entry(channel).Property(x => x.RowVersion).OriginalValue = rowVersion;

    public void SetOriginalRowVersion(ChannelBrand brand, byte[] rowVersion) =>
        dbContext.Entry(brand).Property(x => x.RowVersion).OriginalValue = rowVersion;
}
