using HistoriasPaolin.Domain.Common;

namespace HistoriasPaolin.Domain.Episodes;

public sealed class EpisodeScene : AuditableEntity
{
    public Guid EpisodeId { get; set; }
    public Episode? Episode { get; set; }
    public int SortOrder { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string Status { get; set; } = EpisodeStatuses.Draft;
}
