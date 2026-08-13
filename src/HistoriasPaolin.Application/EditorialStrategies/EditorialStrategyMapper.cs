using HistoriasPaolin.Contracts.EditorialStrategies;
using HistoriasPaolin.Domain.Channels;

namespace HistoriasPaolin.Application.EditorialStrategies;

public static class EditorialStrategyMapper
{
    public static EditorialStrategySummaryDto ToSummary(EditorialStrategy strategy) =>
        new(strategy.Id, strategy.ChannelId, strategy.Name, strategy.IsActive, strategy.Version, strategy.EffectiveFromUtc, strategy.EffectiveToUtc);

    public static EditorialStrategyDetailDto ToDetail(EditorialStrategy strategy) =>
        new(
            strategy.Id,
            strategy.ChannelId,
            strategy.Name,
            strategy.Description,
            strategy.Objective,
            strategy.PrimaryAudience,
            strategy.AgeFrom,
            strategy.AgeTo,
            strategy.PrimaryLanguage,
            strategy.SecondaryLanguage,
            strategy.Country,
            strategy.Tone,
            strategy.EducationalApproach,
            strategy.ContentStyle,
            strategy.StorytellingStyle,
            strategy.DefaultEpisodeDurationSeconds,
            strategy.MinimumEpisodeDurationSeconds,
            strategy.MaximumEpisodeDurationSeconds,
            strategy.ScenesMin,
            strategy.ScenesMax,
            strategy.IsActive,
            strategy.Version,
            strategy.EffectiveFromUtc,
            strategy.EffectiveToUtc,
            Convert.ToBase64String(strategy.RowVersion));

    public static EditorialPillarDto ToPillar(EditorialPillar pillar) =>
        new(pillar.Id, pillar.Code, pillar.Name, pillar.Description, pillar.Weight, pillar.IsActive, pillar.SortOrder);

    public static EditorialTopicDto ToTopic(EditorialTopic topic) =>
        new(topic.Id, topic.Code, topic.Name, topic.Description, topic.Category, topic.Priority, topic.MinAge, topic.MaxAge, topic.IsAllowed, topic.IsActive);

    public static EditorialRestrictionDto ToRestriction(EditorialRestriction restriction) =>
        new(restriction.Id, restriction.RestrictionType, restriction.Code, restriction.Description, restriction.Severity, restriction.IsBlocking, restriction.IsActive);
}
