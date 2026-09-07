namespace HistoriasPaolin.Contracts.Channels;

public sealed record AuditMetadataDto(DateTime CreatedAtUtc, string CreatedBy, DateTime? UpdatedAtUtc, string? UpdatedBy);

public sealed record ChannelSummaryDto(
    Guid Id,
    string Code,
    string Name,
    string Language,
    string Country,
    bool IsActive,
    bool IsMadeForKids,
    string DefaultPublicationPrivacy);

public sealed record ChannelDetailDto(
    Guid Id,
    string Code,
    string Name,
    string Description,
    string Language,
    string Country,
    string TimeZone,
    bool IsActive,
    bool IsMadeForKids,
    string DefaultAspectRatio,
    int DefaultVideoDurationSeconds,
    string DefaultPublicationPrivacy,
    string RowVersion,
    AuditMetadataDto Audit,
    ChannelBrandDto? ActiveBrand);

public sealed record CreateChannelRequest(
    string Code,
    string Name,
    string Description,
    string Language,
    string Country,
    string TimeZone,
    bool IsMadeForKids,
    string DefaultAspectRatio,
    int DefaultVideoDurationSeconds,
    string DefaultPublicationPrivacy);

public sealed record UpdateChannelRequest(
    string Name,
    string Description,
    string Language,
    string Country,
    string TimeZone,
    bool IsMadeForKids,
    string DefaultAspectRatio,
    int DefaultVideoDurationSeconds,
    string DefaultPublicationPrivacy,
    string RowVersion);

public sealed record UpdateChannelStatusRequest(bool IsActive, string RowVersion);

public sealed record ChannelBrandDto(
    Guid Id,
    Guid ChannelId,
    string DisplayName,
    string ShortDescription,
    string LongDescription,
    string PrimaryLanguage,
    string VisualStyle,
    string ToneOfVoice,
    string TargetAudience,
    int TargetAgeFrom,
    int TargetAgeTo,
    string BrandPrompt,
    string CharacterConsistencyPrompt,
    string NegativePrompt,
    bool IsActive,
    string RowVersion,
    AuditMetadataDto Audit);

public sealed record UpdateChannelBrandRequest(
    string DisplayName,
    string ShortDescription,
    string LongDescription,
    string PrimaryLanguage,
    string VisualStyle,
    string ToneOfVoice,
    string TargetAudience,
    int TargetAgeFrom,
    int TargetAgeTo,
    string BrandPrompt,
    string CharacterConsistencyPrompt,
    string NegativePrompt,
    bool IsActive,
    string? RowVersion);
