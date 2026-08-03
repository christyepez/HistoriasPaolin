using HistoriasPaolin.Domain.Episodes;

namespace HistoriasPaolin.Application.Episodes;

public interface IEpisodeRepository
{
    Task<Episode?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Episode episode, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
