using HistoriasPaolin.Domain.Episodes;

namespace HistoriasPaolin.Application.Episodes;

public interface IEpisodeRepository
{
    Task<IReadOnlyList<Episode>> ListAsync(int take, CancellationToken cancellationToken);
    Task<Episode?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Episode episode, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
