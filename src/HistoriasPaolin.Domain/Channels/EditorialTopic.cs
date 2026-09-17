using HistoriasPaolin.Domain.Common;

namespace HistoriasPaolin.Domain.Channels;

public sealed class EditorialTopic : AuditableEntity
{
    public Guid EditorialStrategyId { get; set; }
    public EditorialStrategy? EditorialStrategy { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public bool IsAllowed { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
