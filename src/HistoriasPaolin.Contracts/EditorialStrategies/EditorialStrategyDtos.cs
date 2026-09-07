namespace HistoriasPaolin.Contracts.EditorialStrategies;

public sealed record AuditMetadataDto(DateTime CreatedAtUtc, string CreatedBy, DateTime? UpdatedAtUtc, string? UpdatedBy);

public sealed record EditorialStrategySummaryDto(Guid Id, Guid ChannelId, string Name, bool IsActive, int Version, DateTime EffectiveFromUtc, DateTime? EffectiveToUtc);

public sealed record EditorialStrategyDetailDto(
    Guid Id,
    Guid ChannelId,
    string Name,
    string Description,
    string Objective,
    string PrimaryAudience,
    int AgeFrom,
    int AgeTo,
    string PrimaryLanguage,
    string SecondaryLanguage,
    string Country,
    string Tone,
    string EducationalApproach,
    string ContentStyle,
    string StorytellingStyle,
    int DefaultEpisodeDurationSeconds,
    int MinimumEpisodeDurationSeconds,
    int MaximumEpisodeDurationSeconds,
    int ScenesMin,
    int ScenesMax,
    bool IsActive,
    int Version,
    DateTime EffectiveFromUtc,
    DateTime? EffectiveToUtc,
    string RowVersion,
    AuditMetadataDto Audit);

public sealed record CreateEditorialStrategyRequest(
    string Name,
    string Description,
    string Objective,
    string PrimaryAudience,
    int AgeFrom,
    int AgeTo,
    string PrimaryLanguage,
    string SecondaryLanguage,
    string Country,
    string Tone,
    string EducationalApproach,
    string ContentStyle,
    string StorytellingStyle,
    int DefaultEpisodeDurationSeconds,
    int MinimumEpisodeDurationSeconds,
    int MaximumEpisodeDurationSeconds,
    int ScenesMin,
    int ScenesMax,
    bool IsActive,
    DateTime EffectiveFromUtc,
    DateTime? EffectiveToUtc);

public sealed record UpdateEditorialStrategyRequest(
    string Name,
    string Description,
    string Objective,
    string PrimaryAudience,
    int AgeFrom,
    int AgeTo,
    string PrimaryLanguage,
    string SecondaryLanguage,
    string Country,
    string Tone,
    string EducationalApproach,
    string ContentStyle,
    string StorytellingStyle,
    int DefaultEpisodeDurationSeconds,
    int MinimumEpisodeDurationSeconds,
    int MaximumEpisodeDurationSeconds,
    int ScenesMin,
    int ScenesMax,
    DateTime EffectiveFromUtc,
    DateTime? EffectiveToUtc,
    string RowVersion);

public sealed record EditorialPillarDto(Guid Id, string Code, string Name, string Description, int Weight, bool IsActive, int SortOrder);

public sealed record UpdateEditorialPillarsRequest(IReadOnlyList<EditorialPillarDto> Pillars, string RowVersion);

public sealed record EditorialTopicDto(Guid Id, string Code, string Name, string Description, string Category, int Priority, int MinAge, int MaxAge, bool IsAllowed, bool IsActive);

public sealed record UpdateEditorialTopicsRequest(IReadOnlyList<EditorialTopicDto> Topics, string RowVersion);

public sealed record EditorialRestrictionDto(Guid Id, string RestrictionType, string Code, string Description, string Severity, bool IsBlocking, bool IsActive);

public sealed record UpdateEditorialRestrictionsRequest(IReadOnlyList<EditorialRestrictionDto> Restrictions, string RowVersion);
