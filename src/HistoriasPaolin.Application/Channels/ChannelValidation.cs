using System.Text.RegularExpressions;
using HistoriasPaolin.Contracts.Channels;
using HistoriasPaolin.Domain.Channels;

namespace HistoriasPaolin.Application.Channels;

public static partial class ChannelValidation
{
    public const int MaxDefaultDurationSeconds = 3600;

    public static void Validate(CreateChannelRequest request)
    {
        var errors = ValidateChannel(
            request.Code,
            request.Name,
            request.Language,
            request.Country,
            request.TimeZone,
            request.DefaultVideoDurationSeconds,
            request.DefaultPublicationPrivacy);

        ThrowIfAny(errors);
    }

    public static void Validate(UpdateChannelRequest request)
    {
        var errors = ValidateChannel(
            "valid-code",
            request.Name,
            request.Language,
            request.Country,
            request.TimeZone,
            request.DefaultVideoDurationSeconds,
            request.DefaultPublicationPrivacy);
        ValidateRowVersion(request.RowVersion, errors);

        ThrowIfAny(errors);
    }

    public static void Validate(UpdateChannelStatusRequest request)
    {
        var errors = new List<string>();
        ValidateRowVersion(request.RowVersion, errors);
        ThrowIfAny(errors);
    }

    public static void Validate(UpdateChannelBrandRequest request)
    {
        var errors = new List<string>();
        Required(request.VisualStyle, nameof(request.VisualStyle), errors);
        Required(request.ToneOfVoice, nameof(request.ToneOfVoice), errors);
        Required(request.TargetAudience, nameof(request.TargetAudience), errors);
        Required(request.BrandPrompt, nameof(request.BrandPrompt), errors);
        Required(request.NegativePrompt, nameof(request.NegativePrompt), errors);

        if (request.TargetAgeFrom < 0)
        {
            errors.Add("TargetAgeFrom must be greater than or equal to 0.");
        }

        if (request.TargetAgeTo < request.TargetAgeFrom)
        {
            errors.Add("TargetAgeTo must be greater than or equal to TargetAgeFrom.");
        }

        if (request.RowVersion is not null)
        {
            ValidateRowVersion(request.RowVersion, errors);
        }

        ThrowIfAny(errors);
    }

    private static List<string> ValidateChannel(
        string code,
        string name,
        string language,
        string country,
        string timeZone,
        int defaultVideoDurationSeconds,
        string privacy)
    {
        var errors = new List<string>();
        Required(code, nameof(code), errors);
        if (!string.IsNullOrWhiteSpace(code) && !CodeRegex().IsMatch(code))
        {
            errors.Add("Code must use kebab-case.");
        }

        if (code.Length > 100)
        {
            errors.Add("Code must be 100 characters or fewer.");
        }

        Required(name, nameof(name), errors);
        if (name.Length > 200)
        {
            errors.Add("Name must be 200 characters or fewer.");
        }

        Required(language, nameof(language), errors);
        Required(country, nameof(country), errors);
        Required(timeZone, nameof(timeZone), errors);

        if (defaultVideoDurationSeconds <= 0)
        {
            errors.Add("DefaultVideoDurationSeconds must be greater than 0.");
        }

        if (defaultVideoDurationSeconds > MaxDefaultDurationSeconds)
        {
            errors.Add($"DefaultVideoDurationSeconds must be less than or equal to {MaxDefaultDurationSeconds}.");
        }

        if (!ChannelPublicationPrivacy.Allowed.Contains(privacy))
        {
            errors.Add("DefaultPublicationPrivacy must be private, unlisted or public.");
        }

        return errors;
    }

    private static void Required(string value, string name, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{name} is required.");
        }
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

    private static void ThrowIfAny(IReadOnlyList<string> errors)
    {
        if (errors.Count > 0)
        {
            throw new ChannelValidationException(errors);
        }
    }

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex CodeRegex();
}
