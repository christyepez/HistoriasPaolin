using HistoriasPaolin.Contracts.Channels;
using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Integration;

namespace HistoriasPaolin.Application.Channels;

public sealed class ChannelService(IChannelRepository repository) : IChannelService
{
    public async Task<IReadOnlyList<ChannelSummaryDto>> ListAsync(CancellationToken cancellationToken)
    {
        var channels = await repository.ListAsync(cancellationToken);
        return channels.Select(ChannelMapper.ToSummary).ToList();
    }

    public async Task<ChannelDetailDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var channel = await repository.GetAsync(id, cancellationToken) ?? throw new ChannelNotFoundException(id);
        return ChannelMapper.ToDetail(channel);
    }

    public async Task<ChannelDetailDto> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var channel = await repository.GetByCodeAsync(code, cancellationToken) ?? throw new ChannelCodeNotFoundException(code);
        return ChannelMapper.ToDetail(channel);
    }

    public async Task<ChannelDetailDto> CreateAsync(CreateChannelRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        ChannelValidation.Validate(request);
        if (await repository.CodeExistsAsync(request.Code, null, cancellationToken))
        {
            throw new ChannelDuplicateCodeException(request.Code);
        }

        var channel = new Channel
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Language = request.Language,
            Country = request.Country,
            TimeZone = request.TimeZone,
            IsMadeForKids = request.IsMadeForKids,
            DefaultAspectRatio = request.DefaultAspectRatio,
            DefaultVideoDurationSeconds = request.DefaultVideoDurationSeconds,
            DefaultPublicationPrivacy = request.DefaultPublicationPrivacy
        };

        await repository.AddAsync(channel, cancellationToken);
        AddAudit(channel, "created", actor, correlationId);
        await SaveAsync(cancellationToken);
        return ChannelMapper.ToDetail(channel);
    }

    public async Task<ChannelDetailDto> UpdateAsync(Guid id, UpdateChannelRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        ChannelValidation.Validate(request);
        var channel = await repository.GetAsync(id, cancellationToken) ?? throw new ChannelNotFoundException(id);
        repository.SetOriginalRowVersion(channel, Convert.FromBase64String(request.RowVersion));

        channel.Name = request.Name;
        channel.Description = request.Description;
        channel.Language = request.Language;
        channel.Country = request.Country;
        channel.TimeZone = request.TimeZone;
        channel.IsMadeForKids = request.IsMadeForKids;
        channel.DefaultAspectRatio = request.DefaultAspectRatio;
        channel.DefaultVideoDurationSeconds = request.DefaultVideoDurationSeconds;
        channel.DefaultPublicationPrivacy = request.DefaultPublicationPrivacy;

        AddAudit(channel, "updated", actor, correlationId);
        await SaveAsync(cancellationToken);
        return ChannelMapper.ToDetail(channel);
    }

    public async Task<ChannelDetailDto> UpdateStatusAsync(Guid id, UpdateChannelStatusRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        ChannelValidation.Validate(request);
        var channel = await repository.GetAsync(id, cancellationToken) ?? throw new ChannelNotFoundException(id);
        repository.SetOriginalRowVersion(channel, Convert.FromBase64String(request.RowVersion));
        channel.IsActive = request.IsActive;

        AddAudit(channel, request.IsActive ? "activated" : "deactivated", actor, correlationId);
        await SaveAsync(cancellationToken);
        return ChannelMapper.ToDetail(channel);
    }

    public async Task<ChannelBrandDto> GetBrandAsync(Guid id, CancellationToken cancellationToken)
    {
        var channel = await repository.GetAsync(id, cancellationToken) ?? throw new ChannelNotFoundException(id);
        return channel.ActiveBrand is null
            ? throw new ChannelNotFoundException(id)
            : ChannelMapper.ToBrand(channel.ActiveBrand);
    }

    public async Task<ChannelBrandDto> UpdateBrandAsync(Guid id, UpdateChannelBrandRequest request, string actor, string correlationId, CancellationToken cancellationToken)
    {
        ChannelValidation.Validate(request);
        var channel = await repository.GetAsync(id, cancellationToken) ?? throw new ChannelNotFoundException(id);
        var brand = channel.ActiveBrand ?? new ChannelBrand { ChannelId = channel.Id };

        if (request.RowVersion is not null && brand.Id != Guid.Empty)
        {
            repository.SetOriginalRowVersion(brand, Convert.FromBase64String(request.RowVersion));
        }

        brand.DisplayName = request.DisplayName;
        brand.ShortDescription = request.ShortDescription;
        brand.LongDescription = request.LongDescription;
        brand.PrimaryLanguage = request.PrimaryLanguage;
        brand.VisualStyle = request.VisualStyle;
        brand.ToneOfVoice = request.ToneOfVoice;
        brand.TargetAudience = request.TargetAudience;
        brand.TargetAgeFrom = request.TargetAgeFrom;
        brand.TargetAgeTo = request.TargetAgeTo;
        brand.BrandPrompt = request.BrandPrompt;
        brand.CharacterConsistencyPrompt = request.CharacterConsistencyPrompt;
        brand.NegativePrompt = request.NegativePrompt;
        brand.IsActive = request.IsActive;

        channel.SetBrand(brand);
        if (channel.Brands.Count(existingBrand => existingBrand.IsActive) > 1)
        {
            throw new ChannelValidationException(["Only one ChannelBrand can be active per Channel."]);
        }

        AddAudit(channel, "brand-updated", actor, correlationId);
        await SaveAsync(cancellationToken);
        return ChannelMapper.ToBrand(brand);
    }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        await repository.SaveChangesAsync(cancellationToken);
    }

    private void AddAudit(Channel channel, string action, string actor, string correlationId)
    {
        repository.AddOutboxMessage(new OutboxMessage
        {
            TenantId = "default",
            AggregateType = nameof(Channel),
            AggregateId = channel.Id.ToString(),
            EventType = $"HistoriasPaolin.Channel.{action}.v1",
            PayloadJson = $$"""{"channelId":"{{channel.Id}}","code":"{{channel.Code}}","actor":"{{actor}}","action":"{{action}}"}""",
            CorrelationId = correlationId,
            IdempotencyKey = $"channel:{action}:{channel.Id}:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"
        });
    }
}
