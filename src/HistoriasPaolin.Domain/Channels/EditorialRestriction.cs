using HistoriasPaolin.Domain.Common;

namespace HistoriasPaolin.Domain.Channels;

public sealed class EditorialRestriction : AuditableEntity
{
    public Guid EditorialStrategyId { get; set; }
    public EditorialStrategy? EditorialStrategy { get; set; }
    public string RestrictionType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public bool IsBlocking { get; set; }
    public bool IsActive { get; set; } = true;
}
