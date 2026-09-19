using HistoriasPaolin.Domain.Common;

namespace HistoriasPaolin.Domain.Channels;

public sealed class EditorialPillar : AuditableEntity
{
    public Guid EditorialStrategyId { get; set; }
    public EditorialStrategy? EditorialStrategy { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Weight { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
