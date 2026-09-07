using HistoriasPaolin.Contracts.Channels;
using HistoriasPaolin.Domain.Channels;

namespace HistoriasPaolin.Application.Channels;

public static class ChannelMapper
{
    public static ChannelSummaryDto ToSummary(Channel channel) =>
        new(
            channel.Id,
            channel.Code,
            channel.Name,
            channel.Language,
            channel.Country,
            channel.IsActive,
            channel.IsMadeForKids,
            channel.DefaultPublicationPrivacy);

    public static ChannelDetailDto ToDetail(Channel channel) =>
        new(
            channel.Id,
            channel.Code,
            channel.Name,
            channel.Description,
            channel.Language,
            channel.Country,
            channel.TimeZone,
            channel.IsActive,
            channel.IsMadeForKids,
            channel.DefaultAspectRatio,
            channel.DefaultVideoDurationSeconds,
            channel.DefaultPublicationPrivacy,
            Convert.ToBase64String(channel.RowVersion),
            ToAudit(channel),
            channel.ActiveBrand is null ? null : ToBrand(channel.ActiveBrand));

    public static ChannelBrandDto ToBrand(ChannelBrand brand) =>
        new(
            brand.Id,
            brand.ChannelId,
            brand.DisplayName,
            brand.ShortDescription,
            brand.LongDescription,
            brand.PrimaryLanguage,
            brand.VisualStyle,
            brand.ToneOfVoice,
            brand.TargetAudience,
            brand.TargetAgeFrom,
            brand.TargetAgeTo,
            brand.BrandPrompt,
            brand.CharacterConsistencyPrompt,
            brand.NegativePrompt,
            brand.IsActive,
            Convert.ToBase64String(brand.RowVersion),
            ToAudit(brand));

    private static AuditMetadataDto ToAudit(Channel channel) =>
        new(channel.CreatedAtUtc, channel.CreatedBy, channel.UpdatedAtUtc, channel.UpdatedBy);

    private static AuditMetadataDto ToAudit(ChannelBrand brand) =>
        new(brand.CreatedAtUtc, brand.CreatedBy, brand.UpdatedAtUtc, brand.UpdatedBy);
}
