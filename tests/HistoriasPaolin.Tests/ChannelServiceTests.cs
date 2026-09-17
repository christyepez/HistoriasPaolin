using HistoriasPaolin.Application.Channels;
using HistoriasPaolin.Contracts.Channels;
using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Integration;

namespace HistoriasPaolin.Tests;

public sealed class ChannelServiceTests
{
    [Fact]
    public async Task CreateChannelAcceptsValidRequest()
    {
        var service = new ChannelService(new FakeChannelRepository());

        var channel = await service.CreateAsync(ValidCreateRequest(), "tester", "corr-1", CancellationToken.None);

        Assert.Equal("historias-paolin", channel.Code);
    }

    [Fact]
    public async Task CreateChannelRejectsDuplicateCode()
    {
        var repository = new FakeChannelRepository();
        repository.Channels.Add(new Channel { Code = "historias-paolin", Name = "Existing" });
        var service = new ChannelService(repository);

        await Assert.ThrowsAsync<ChannelDuplicateCodeException>(() =>
            service.CreateAsync(ValidCreateRequest(), "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task CreateChannelRejectsInvalidCode()
    {
        var request = ValidCreateRequest() with { Code = "Historias Paolin" };
        var service = new ChannelService(new FakeChannelRepository());

        await Assert.ThrowsAsync<ChannelValidationException>(() =>
            service.CreateAsync(request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task CreateChannelRejectsInvalidPrivacy()
    {
        var request = ValidCreateRequest() with { DefaultPublicationPrivacy = "friends" };
        var service = new ChannelService(new FakeChannelRepository());

        await Assert.ThrowsAsync<ChannelValidationException>(() =>
            service.CreateAsync(request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task CreateChannelRejectsInvalidDuration()
    {
        var request = ValidCreateRequest() with { DefaultVideoDurationSeconds = 0 };
        var service = new ChannelService(new FakeChannelRepository());

        await Assert.ThrowsAsync<ChannelValidationException>(() =>
            service.CreateAsync(request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task UpdateBrandAcceptsValidBrand()
    {
        var repository = new FakeChannelRepository();
        var channel = ExistingChannel();
        repository.Channels.Add(channel);
        var service = new ChannelService(repository);

        var brand = await service.UpdateBrandAsync(channel.Id, ValidBrandRequest(), "tester", "corr-1", CancellationToken.None);

        Assert.Equal("Familias con ninos pequenos", brand.TargetAudience);
    }

    [Fact]
    public async Task UpdateBrandRejectsInvalidAgeRange()
    {
        var repository = new FakeChannelRepository();
        var channel = ExistingChannel();
        repository.Channels.Add(channel);
        var service = new ChannelService(repository);
        var request = ValidBrandRequest() with { TargetAgeFrom = 7, TargetAgeTo = 2 };

        await Assert.ThrowsAsync<ChannelValidationException>(() =>
            service.UpdateBrandAsync(channel.Id, request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public void ChannelAllowsOnlyOneActiveBrand()
    {
        var channel = ExistingChannel();
        channel.Brands.Add(new ChannelBrand { ChannelId = channel.Id, IsActive = true });

        channel.SetBrand(new ChannelBrand { ChannelId = channel.Id, IsActive = true });

        Assert.Single(channel.Brands, brand => brand.IsActive);
    }

    [Fact]
    public async Task UpdateChannelPropagatesConcurrencyConflict()
    {
        var repository = new FakeChannelRepository { ThrowConcurrency = true };
        var channel = ExistingChannel();
        repository.Channels.Add(channel);
        var service = new ChannelService(repository);

        await Assert.ThrowsAsync<ChannelConcurrencyException>(() =>
            service.UpdateAsync(channel.Id, ValidUpdateRequest(channel), "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task UpdateStatusCanDeactivateChannel()
    {
        var repository = new FakeChannelRepository();
        var channel = ExistingChannel();
        repository.Channels.Add(channel);
        var service = new ChannelService(repository);

        var updated = await service.UpdateStatusAsync(channel.Id, new UpdateChannelStatusRequest(false, Convert.ToBase64String(channel.RowVersion)), "tester", "corr-1", CancellationToken.None);

        Assert.False(updated.IsActive);
    }

    private static CreateChannelRequest ValidCreateRequest() =>
        new("historias-paolin", "Historias de Paolin", "Canal infantil", "es", "EC", "America/Guayaquil", true, "16:9", 180, "private");

    private static UpdateChannelRequest ValidUpdateRequest(Channel channel) =>
        new("Historias de Paolin", "Canal infantil", "es", "EC", "America/Guayaquil", true, "16:9", 180, "private", Convert.ToBase64String(channel.RowVersion));

    private static UpdateChannelBrandRequest ValidBrandRequest() =>
        new(
            "Historias de Paolin",
            "Historias infantiles",
            "Historias infantiles educativas y seguras",
            "es",
            "Colorido y amable",
            "Calido y curioso",
            "Familias con ninos pequenos",
            2,
            6,
            "Crear historias originales para primera infancia.",
            "Mantener consistencia de personajes.",
            "Sin violencia ni franquicias.",
            true,
            null);

    private static Channel ExistingChannel() =>
        new()
        {
            Code = "historias-paolin",
            Name = "Historias de Paolin",
            Description = "Canal infantil",
            Language = "es",
            Country = "EC",
            TimeZone = "America/Guayaquil",
            RowVersion = [1, 2, 3]
        };

    private sealed class FakeChannelRepository : IChannelRepository
    {
        public List<Channel> Channels { get; } = [];
        public bool ThrowConcurrency { get; init; }

        public Task<IReadOnlyList<Channel>> ListAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Channel>>(Channels);

        public Task<Channel?> GetAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Channels.SingleOrDefault(channel => channel.Id == id));

        public Task<Channel?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
            Task.FromResult(Channels.SingleOrDefault(channel => channel.Code == code));

        public Task<bool> CodeExistsAsync(string code, Guid? exceptId, CancellationToken cancellationToken) =>
            Task.FromResult(Channels.Any(channel => channel.Code == code && (!exceptId.HasValue || channel.Id != exceptId.Value)));

        public Task AddAsync(Channel channel, CancellationToken cancellationToken)
        {
            channel.RowVersion = [1, 2, 3];
            Channels.Add(channel);
            return Task.CompletedTask;
        }

        public void AddOutboxMessage(OutboxMessage message)
        {
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (ThrowConcurrency)
            {
                throw new ChannelConcurrencyException();
            }

            return Task.CompletedTask;
        }

        public void SetOriginalRowVersion(Channel channel, byte[] rowVersion)
        {
        }

        public void SetOriginalRowVersion(ChannelBrand brand, byte[] rowVersion)
        {
        }
    }
}
