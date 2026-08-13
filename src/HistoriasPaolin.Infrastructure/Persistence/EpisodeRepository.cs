using HistoriasPaolin.Application.Episodes;
using HistoriasPaolin.Domain.Episodes;
using Microsoft.EntityFrameworkCore;

namespace HistoriasPaolin.Infrastructure.Persistence;

public sealed class EpisodeRepository(HistoriasPaolinDbContext dbContext) : IEpisodeRepository
{
    public async Task<IReadOnlyList<Episode>> ListAsync(int take, CancellationToken cancellationToken) =>
        await dbContext.Episodes
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

    public Task<Episode?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Episodes.Include(x => x.Scenes).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Episode episode, CancellationToken cancellationToken) =>
        await dbContext.Episodes.AddAsync(episode, cancellationToken);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
