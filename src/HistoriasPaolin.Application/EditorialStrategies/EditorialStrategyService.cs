using HistoriasPaolin.Contracts.EditorialStrategies;
using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Integration;

namespace HistoriasPaolin.Application.EditorialStrategies;

public sealed class EditorialStrategyService(IEditorialStrategyRepository repository) : IEditorialStrategyService
{
    public async Task<IReadOnlyList<EditorialStrategySummaryDto>> ListAsync(Guid channelId, CancellationToken cancellationToken)
    {
        _ = await repository.GetChannelAsync(channelId, cancellationToken) ?? throw new EditorialStrategyChannelNotFoundException(channelId);
        var strategies = await repository.ListByChannelAsync(channelId, cancellationToken);
        return strategies.Select(EditorialStrategyMapper.ToSummary).ToList();
    }

    public async Task<EditorialStrategyDetailDto> GetAsync(Guid id, CancellationToken cancellationToken) =>
        EditorialStrategyMapper.ToDetail(await GetRequiredAsync(id, cancellationToken));

    public async Task<EditorialStrategyDetailDto> GetActiveAsync(Guid channelId, CancellationToken cancellationToken)
    {
        _ = await repository.GetChannelAsync(channelId, cancellationToken) ?? throw new EditorialStrategyChannelNotFoundException(channelId);
        var strategy = await repository.GetActiveAsync(channelId, DateTime.UtcNow, cancellationToken) ?? throw new EditorialStrategyActiveNotFoundException(channelId);
        return EditorialStrategyMapper.ToDetail(strategy);
    }

    public async Task<EditorialStrategyDetailDto> CreateAsync(Guid channelId, CreateEditorialStrategyRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        EditorialStrategyValidation.Validate(request);
        var channel = await repository.GetChannelAsync(channelId, cancellationToken) ?? throw new EditorialStrategyChannelNotFoundException(channelId);
        var strategy = new EditorialStrategy
        {
            ChannelId = channel.Id,
            Name = request.Name,
            Description = request.Description,
            Objective = request.Objective,
            PrimaryAudience = request.PrimaryAudience,
            AgeFrom = request.AgeFrom,
            AgeTo = request.AgeTo,
            PrimaryLanguage = request.PrimaryLanguage,
            SecondaryLanguage = request.SecondaryLanguage,
            Country = request.Country,
            Tone = request.Tone,
            EducationalApproach = request.EducationalApproach,
            ContentStyle = request.ContentStyle,
            StorytellingStyle = request.StorytellingStyle,
            DefaultEpisodeDurationSeconds = request.DefaultEpisodeDurationSeconds,
            MinimumEpisodeDurationSeconds = request.MinimumEpisodeDurationSeconds,
            MaximumEpisodeDurationSeconds = request.MaximumEpisodeDurationSeconds,
            ScenesMin = request.ScenesMin,
            ScenesMax = request.ScenesMax,
            IsActive = request.IsActive,
            EffectiveFromUtc = request.EffectiveFromUtc,
            EffectiveToUtc = request.EffectiveToUtc
        };

        if (strategy.IsActive)
        {
            EditorialStrategyValidation.EnsureCanActivate(channel, strategy);
        }

        await repository.AddAsync(strategy, cancellationToken);
        AddAudit(strategy, "created", actor, correlationId);
        await repository.SaveChangesAsync(cancellationToken);
        return EditorialStrategyMapper.ToDetail(strategy);
    }

    public async Task<EditorialStrategyDetailDto> UpdateAsync(Guid id, UpdateEditorialStrategyRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        EditorialStrategyValidation.Validate(request);
        var strategy = await GetRequiredAsync(id, cancellationToken);
        repository.SetOriginalRowVersion(strategy, Convert.FromBase64String(request.RowVersion));

        strategy.Name = request.Name;
        strategy.Description = request.Description;
        strategy.Objective = request.Objective;
        strategy.PrimaryAudience = request.PrimaryAudience;
        strategy.AgeFrom = request.AgeFrom;
        strategy.AgeTo = request.AgeTo;
        strategy.PrimaryLanguage = request.PrimaryLanguage;
        strategy.SecondaryLanguage = request.SecondaryLanguage;
        strategy.Country = request.Country;
        strategy.Tone = request.Tone;
        strategy.EducationalApproach = request.EducationalApproach;
        strategy.ContentStyle = request.ContentStyle;
        strategy.StorytellingStyle = request.StorytellingStyle;
        strategy.DefaultEpisodeDurationSeconds = request.DefaultEpisodeDurationSeconds;
        strategy.MinimumEpisodeDurationSeconds = request.MinimumEpisodeDurationSeconds;
        strategy.MaximumEpisodeDurationSeconds = request.MaximumEpisodeDurationSeconds;
        strategy.ScenesMin = request.ScenesMin;
        strategy.ScenesMax = request.ScenesMax;
        strategy.EffectiveFromUtc = request.EffectiveFromUtc;
        strategy.EffectiveToUtc = request.EffectiveToUtc;
        strategy.Version++;

        AddAudit(strategy, "updated", actor, correlationId);
        await repository.SaveChangesAsync(cancellationToken);
        return EditorialStrategyMapper.ToDetail(strategy);
    }

    public async Task<EditorialStrategyDetailDto> ActivateAsync(Guid id, string actor, string correlationId, CancellationToken cancellationToken)
    {
        var strategy = await GetRequiredAsync(id, cancellationToken);
        var channel = strategy.Channel ?? throw new EditorialStrategyChannelNotFoundException(strategy.ChannelId);
        EditorialStrategyValidation.EnsureCanActivate(channel, strategy);
        strategy.IsActive = true;
        AddAudit(strategy, "activated", actor, correlationId);
        await repository.SaveChangesAsync(cancellationToken);
        return EditorialStrategyMapper.ToDetail(strategy);
    }

    public async Task<EditorialStrategyDetailDto> DeactivateAsync(Guid id, string actor, string correlationId, CancellationToken cancellationToken)
    {
        var strategy = await GetRequiredAsync(id, cancellationToken);
        strategy.IsActive = false;
        AddAudit(strategy, "deactivated", actor, correlationId);
        await repository.SaveChangesAsync(cancellationToken);
        return EditorialStrategyMapper.ToDetail(strategy);
    }

    public async Task<IReadOnlyList<EditorialPillarDto>> GetPillarsAsync(Guid id, CancellationToken cancellationToken)
    {
        var strategy = await GetRequiredAsync(id, cancellationToken);
        return strategy.Pillars.OrderBy(pillar => pillar.SortOrder).Select(EditorialStrategyMapper.ToPillar).ToList();
    }

    public async Task<IReadOnlyList<EditorialPillarDto>> UpdatePillarsAsync(Guid id, UpdateEditorialPillarsRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        EditorialStrategyValidation.Validate(request);
        var strategy = await GetRequiredAsync(id, cancellationToken);
        repository.SetOriginalRowVersion(strategy, Convert.FromBase64String(request.RowVersion));
        strategy.Pillars.Clear();
        foreach (var dto in request.Pillars)
        {
            strategy.Pillars.Add(new EditorialPillar
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                Weight = dto.Weight,
                IsActive = dto.IsActive,
                SortOrder = dto.SortOrder
            });
        }

        AddAudit(strategy, "pillars-updated", actor, correlationId);
        await repository.SaveChangesAsync(cancellationToken);
        return strategy.Pillars.OrderBy(pillar => pillar.SortOrder).Select(EditorialStrategyMapper.ToPillar).ToList();
    }

    public async Task<IReadOnlyList<EditorialTopicDto>> GetTopicsAsync(Guid id, CancellationToken cancellationToken)
    {
        var strategy = await GetRequiredAsync(id, cancellationToken);
        return strategy.Topics.OrderBy(topic => topic.Priority).Select(EditorialStrategyMapper.ToTopic).ToList();
    }

    public async Task<IReadOnlyList<EditorialTopicDto>> UpdateTopicsAsync(Guid id, UpdateEditorialTopicsRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        EditorialStrategyValidation.Validate(request);
        var strategy = await GetRequiredAsync(id, cancellationToken);
        repository.SetOriginalRowVersion(strategy, Convert.FromBase64String(request.RowVersion));
        strategy.Topics.Clear();
        foreach (var dto in request.Topics)
        {
            strategy.Topics.Add(new EditorialTopic
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Priority = dto.Priority,
                MinAge = dto.MinAge,
                MaxAge = dto.MaxAge,
                IsAllowed = dto.IsAllowed,
                IsActive = dto.IsActive
            });
        }

        AddAudit(strategy, "topics-updated", actor, correlationId);
        await repository.SaveChangesAsync(cancellationToken);
        return strategy.Topics.OrderBy(topic => topic.Priority).Select(EditorialStrategyMapper.ToTopic).ToList();
    }

    public async Task<IReadOnlyList<EditorialRestrictionDto>> GetRestrictionsAsync(Guid id, CancellationToken cancellationToken)
    {
        var strategy = await GetRequiredAsync(id, cancellationToken);
        return strategy.Restrictions.OrderBy(restriction => restriction.RestrictionType).ThenBy(restriction => restriction.Code).Select(EditorialStrategyMapper.ToRestriction).ToList();
    }

    public async Task<IReadOnlyList<EditorialRestrictionDto>> UpdateRestrictionsAsync(Guid id, UpdateEditorialRestrictionsRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        EditorialStrategyValidation.Validate(request);
        var strategy = await GetRequiredAsync(id, cancellationToken);
        repository.SetOriginalRowVersion(strategy, Convert.FromBase64String(request.RowVersion));
        strategy.Restrictions.Clear();
        foreach (var dto in request.Restrictions)
        {
            strategy.Restrictions.Add(new EditorialRestriction
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                RestrictionType = dto.RestrictionType,
                Code = dto.Code,
                Description = dto.Description,
                Severity = dto.Severity,
                IsBlocking = dto.IsBlocking,
                IsActive = dto.IsActive
            });
        }

        AddAudit(strategy, "restrictions-updated", actor, correlationId);
        await repository.SaveChangesAsync(cancellationToken);
        return strategy.Restrictions.OrderBy(restriction => restriction.RestrictionType).ThenBy(restriction => restriction.Code).Select(EditorialStrategyMapper.ToRestriction).ToList();
    }

    private async Task<EditorialStrategy> GetRequiredAsync(Guid id, CancellationToken cancellationToken) =>
        await repository.GetAsync(id, cancellationToken) ?? throw new EditorialStrategyNotFoundException(id);

    private void AddAudit(EditorialStrategy strategy, string action, string actor, string correlationId)
    {
        repository.AddOutboxMessage(new OutboxMessage
        {
            TenantId = "default",
            AggregateType = nameof(EditorialStrategy),
            AggregateId = strategy.Id.ToString(),
            EventType = $"HistoriasPaolin.EditorialStrategy.{action}.v1",
            PayloadJson = $$"""{"strategyId":"{{strategy.Id}}","channelId":"{{strategy.ChannelId}}","actor":"{{actor}}","action":"{{action}}"}""",
            CorrelationId = correlationId,
            IdempotencyKey = $"editorial-strategy:{action}:{strategy.Id}:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"
        });
    }
}
