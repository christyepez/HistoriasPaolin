using HistoriasPaolin.Contracts.EditorialStrategies;
using HistoriasPaolin.Domain.Channels;

namespace HistoriasPaolin.Application.EditorialStrategies;

public static class EditorialStrategyValidation
{
    public static void Validate(CreateEditorialStrategyRequest request) =>
        ThrowIfAny(ValidateCore(request.Name, request.Objective, request.PrimaryAudience, request.AgeFrom, request.AgeTo, request.MinimumEpisodeDurationSeconds, request.DefaultEpisodeDurationSeconds, request.MaximumEpisodeDurationSeconds, request.ScenesMin, request.ScenesMax, request.EffectiveFromUtc, request.EffectiveToUtc));

    public static void Validate(UpdateEditorialStrategyRequest request)
    {
        var errors = ValidateCore(request.Name, request.Objective, request.PrimaryAudience, request.AgeFrom, request.AgeTo, request.MinimumEpisodeDurationSeconds, request.DefaultEpisodeDurationSeconds, request.MaximumEpisodeDurationSeconds, request.ScenesMin, request.ScenesMax, request.EffectiveFromUtc, request.EffectiveToUtc);
        ValidateRowVersion(request.RowVersion, errors);
        ThrowIfAny(errors);
    }

    public static void Validate(UpdateEditorialPillarsRequest request)
    {
        var errors = new List<string>();
        ValidateRowVersion(request.RowVersion, errors);
        var duplicateCodes = request.Pillars.GroupBy(pillar => pillar.Code).Where(group => group.Count() > 1).Select(group => group.Key);
        errors.AddRange(duplicateCodes.Select(code => $"Pillar code '{code}' is duplicated."));
        var activeWeight = request.Pillars.Where(pillar => pillar.IsActive).Sum(pillar => pillar.Weight);
        if (activeWeight > 100)
        {
            errors.Add("Active pillar weights cannot exceed 100.");
        }

        foreach (var pillar in request.Pillars)
        {
            Required(pillar.Code, nameof(pillar.Code), errors);
            Required(pillar.Name, nameof(pillar.Name), errors);
            if (pillar.Weight is < 0 or > 100)
            {
                errors.Add("Pillar weight must be between 0 and 100.");
            }
        }

        ThrowIfAny(errors);
    }

    public static void Validate(UpdateEditorialTopicsRequest request)
    {
        var errors = new List<string>();
        ValidateRowVersion(request.RowVersion, errors);
        var duplicateCodes = request.Topics.GroupBy(topic => topic.Code).Where(group => group.Count() > 1).Select(group => group.Key);
        errors.AddRange(duplicateCodes.Select(code => $"Topic code '{code}' is duplicated."));
        foreach (var topic in request.Topics)
        {
            Required(topic.Code, nameof(topic.Code), errors);
            Required(topic.Name, nameof(topic.Name), errors);
            if (topic.MinAge > topic.MaxAge)
            {
                errors.Add("Topic MinAge must be less than or equal to MaxAge.");
            }
        }

        ThrowIfAny(errors);
    }

    public static void Validate(UpdateEditorialRestrictionsRequest request)
    {
        var errors = new List<string>();
        ValidateRowVersion(request.RowVersion, errors);
        foreach (var restriction in request.Restrictions)
        {
            Required(restriction.RestrictionType, nameof(restriction.RestrictionType), errors);
            Required(restriction.Code, nameof(restriction.Code), errors);
            Required(restriction.Description, nameof(restriction.Description), errors);
            Required(restriction.Severity, nameof(restriction.Severity), errors);
        }

        ThrowIfAny(errors);
    }

    public static void EnsureCanActivate(Channel channel, EditorialStrategy strategy)
    {
        if (!channel.IsActive)
        {
            throw new EditorialStrategyInactiveChannelException(channel.Id);
        }

        foreach (var existing in channel.EditorialStrategies.Where(existing => existing.Id != strategy.Id))
        {
            existing.IsActive = false;
        }
    }

    private static List<string> ValidateCore(
        string name,
        string objective,
        string primaryAudience,
        int ageFrom,
        int ageTo,
        int minimumDuration,
        int defaultDuration,
        int maximumDuration,
        int scenesMin,
        int scenesMax,
        DateTime effectiveFromUtc,
        DateTime? effectiveToUtc)
    {
        var errors = new List<string>();
        Required(name, nameof(name), errors);
        Required(objective, nameof(objective), errors);
        Required(primaryAudience, nameof(primaryAudience), errors);
        if (ageFrom > ageTo)
        {
            errors.Add("AgeFrom must be less than or equal to AgeTo.");
        }

        if (minimumDuration > defaultDuration)
        {
            errors.Add("MinimumEpisodeDurationSeconds must be less than or equal to DefaultEpisodeDurationSeconds.");
        }

        if (defaultDuration > maximumDuration)
        {
            errors.Add("DefaultEpisodeDurationSeconds must be less than or equal to MaximumEpisodeDurationSeconds.");
        }

        if (scenesMin > scenesMax)
        {
            errors.Add("ScenesMin must be less than or equal to ScenesMax.");
        }

        if (effectiveToUtc.HasValue && effectiveToUtc.Value <= effectiveFromUtc)
        {
            errors.Add("EffectiveToUtc must be later than EffectiveFromUtc.");
        }

        return errors;
    }

    private static void ValidateRowVersion(string rowVersion, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(rowVersion))
        {
            errors.Add("RowVersion is required.");
            return;
        }

        try
        {
            Convert.FromBase64String(rowVersion);
        }
        catch (FormatException)
        {
            errors.Add("RowVersion must be base64.");
        }
    }

    private static void Required(string value, string name, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{name} is required.");
        }
    }

    private static void ThrowIfAny(IReadOnlyList<string> errors)
    {
        if (errors.Count > 0)
        {
            throw new EditorialStrategyValidationException(errors);
        }
    }
}
