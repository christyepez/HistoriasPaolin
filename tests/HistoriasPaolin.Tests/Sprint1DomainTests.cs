using HistoriasPaolin.Application.Channels;
using HistoriasPaolin.Application.EditorialStrategies;
using HistoriasPaolin.Contracts.Channels;
using HistoriasPaolin.Contracts.EditorialStrategies;
using HistoriasPaolin.Domain.Channels;

namespace HistoriasPaolin.Tests;

public sealed class Sprint1DomainTests
{
    [Fact]
    public void ChannelDefaultsMatchSprint1PublishingPolicy()
    {
        var channel = new Channel();

        Assert.True(channel.IsActive);
        Assert.True(channel.IsMadeForKids);
        Assert.Equal("16:9", channel.DefaultAspectRatio);
        Assert.Equal(180, channel.DefaultVideoDurationSeconds);
        Assert.Equal(ChannelPublicationPrivacy.Private, channel.DefaultPublicationPrivacy);
    }

    [Fact]
    public void SetBrandKeepsOnlyOneActiveBrand()
    {
        var channel = ValidChannel();
        var oldBrand = ValidBrand(Guid.Parse("10510000-0000-4000-8000-000000000001"));
        var newBrand = ValidBrand(Guid.Parse("10510000-0000-4000-8000-000000000002"));

        channel.SetBrand(oldBrand);
        channel.SetBrand(newBrand);

        Assert.False(oldBrand.IsActive);
        Assert.True(newBrand.IsActive);
        Assert.Same(newBrand, channel.ActiveBrand);
    }

    [Fact]
    public void HasActiveEditorialStrategyIgnoresOptionalException()
    {
        var channel = ValidChannel();
        var strategy = ValidStrategy(channel.Id);
        channel.EditorialStrategies.Add(strategy);

        Assert.True(channel.HasActiveEditorialStrategy());
        Assert.False(channel.HasActiveEditorialStrategy(strategy.Id));
    }

    [Fact]
    public void ChannelValidationRejectsRequiredFieldsAndInvalidBoundaries()
    {
        var request = new CreateChannelRequest("", "", "", "", "", "", true, "16:9", 0, "draft");

        var exception = Assert.Throws<ChannelValidationException>(() => ChannelValidation.Validate(request));

        Assert.Contains(exception.Errors, error => error.Contains("code is required.", StringComparison.Ordinal));
        Assert.Contains(exception.Errors, error => error.Contains("name is required.", StringComparison.Ordinal));
        Assert.Contains(exception.Errors, error => error.Contains("DefaultVideoDurationSeconds must be greater than 0.", StringComparison.Ordinal));
        Assert.Contains(exception.Errors, error => error.Contains("DefaultPublicationPrivacy must be private, unlisted or public.", StringComparison.Ordinal));
    }

    [Fact]
    public void EditorialStrategyValidationRejectsInvalidEffectiveDateAndRanges()
    {
        var request = new CreateEditorialStrategyRequest(
            "",
            "",
            "",
            "",
            7,
            6,
            "es",
            "",
            "EC",
            "calido",
            "educativo",
            "aventuras",
            "inicio-cierre",
            50,
            60,
            40,
            10,
            5,
            true,
            new DateTime(2026, 9, 2, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc));

        var exception = Assert.Throws<EditorialStrategyValidationException>(() => EditorialStrategyValidation.Validate(request));

        Assert.Contains(exception.Errors, error => error.Contains("name is required.", StringComparison.Ordinal));
        Assert.Contains(exception.Errors, error => error.Contains("AgeFrom must be less than or equal to AgeTo.", StringComparison.Ordinal));
        Assert.Contains(exception.Errors, error => error.Contains("EffectiveToUtc must be later than EffectiveFromUtc.", StringComparison.Ordinal));
    }

    private static Channel ValidChannel() => new()
    {
        Id = Guid.Parse("10510000-0000-4000-8000-000000000010"),
        Code = "hp105-domain",
        Name = "HP-105 Domain",
        Description = "Canal de dominio",
        Language = "es",
        Country = "EC",
        TimeZone = "America/Guayaquil"
    };

    private static ChannelBrand ValidBrand(Guid id) => new()
    {
        Id = id,
        DisplayName = "Marca",
        ShortDescription = "Corta",
        LongDescription = "Larga",
        PrimaryLanguage = "es",
        VisualStyle = "Colorido",
        ToneOfVoice = "Calido",
        TargetAudience = "Familias",
        BrandPrompt = "Marca",
        CharacterConsistencyPrompt = "Consistencia",
        NegativePrompt = "Sin imitaciones",
        IsActive = true
    };

    private static EditorialStrategy ValidStrategy(Guid channelId) => new()
    {
        Id = Guid.Parse("10510000-0000-4000-8000-000000000020"),
        ChannelId = channelId,
        Name = "Estrategia",
        Objective = "Objetivo",
        PrimaryAudience = "Familias",
        AgeFrom = 2,
        AgeTo = 6,
        IsActive = true,
        EffectiveFromUtc = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc)
    };
}
