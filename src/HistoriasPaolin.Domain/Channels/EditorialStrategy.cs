using HistoriasPaolin.Domain.Common;

namespace HistoriasPaolin.Domain.Channels;

public sealed class EditorialStrategy : AuditableEntity
{
    public Guid ChannelId { get; set; }
    public Channel? Channel { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string PrimaryAudience { get; set; } = string.Empty;
    public int AgeFrom { get; set; }
    public int AgeTo { get; set; }
    public string PrimaryLanguage { get; set; } = string.Empty;
    public string SecondaryLanguage { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Tone { get; set; } = string.Empty;
    public string EducationalApproach { get; set; } = string.Empty;
    public string ContentStyle { get; set; } = string.Empty;
    public string StorytellingStyle { get; set; } = string.Empty;
    public int DefaultEpisodeDurationSeconds { get; set; }
    public int MinimumEpisodeDurationSeconds { get; set; }
    public int MaximumEpisodeDurationSeconds { get; set; }
    public int ScenesMin { get; set; }
    public int ScenesMax { get; set; }
    public bool IsActive { get; set; }
    public int Version { get; set; } = 1;
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    public ICollection<EditorialPillar> Pillars { get; set; } = new List<EditorialPillar>();
    public ICollection<EditorialTopic> Topics { get; set; } = new List<EditorialTopic>();
    public ICollection<EditorialRestriction> Restrictions { get; set; } = new List<EditorialRestriction>();
}
