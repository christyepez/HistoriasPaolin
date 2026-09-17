using HistoriasPaolin.Domain.Common;

namespace HistoriasPaolin.Domain.Channels;

public sealed class ChannelBrand : AuditableEntity
{
    public Guid ChannelId { get; set; }
    public Channel? Channel { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public string PrimaryLanguage { get; set; } = string.Empty;
    public string VisualStyle { get; set; } = string.Empty;
    public string ToneOfVoice { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public int TargetAgeFrom { get; set; }
    public int TargetAgeTo { get; set; }
    public string BrandPrompt { get; set; } = string.Empty;
    public string CharacterConsistencyPrompt { get; set; } = string.Empty;
    public string NegativePrompt { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
