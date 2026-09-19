using HistoriasPaolin.Domain.Common;

namespace HistoriasPaolin.Domain.Episodes;

public sealed class Episode : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = EpisodeStatuses.Draft;
    public decimal EstimatedCostUsd { get; set; }
    public ICollection<EpisodeScene> Scenes { get; set; } = new List<EpisodeScene>();
}
