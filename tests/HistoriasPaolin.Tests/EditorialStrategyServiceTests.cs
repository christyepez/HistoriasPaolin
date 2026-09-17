using HistoriasPaolin.Application.EditorialStrategies;
using HistoriasPaolin.Contracts.EditorialStrategies;
using HistoriasPaolin.Domain.Channels;
using HistoriasPaolin.Domain.Integration;

namespace HistoriasPaolin.Tests;

public sealed class EditorialStrategyServiceTests
{
    [Fact]
    public async Task CreateStrategyAcceptsValidRequest()
    {
        var repository = new FakeEditorialStrategyRepository();
        var service = new EditorialStrategyService(repository);

        var result = await service.CreateAsync(repository.Channel.Id, ValidCreate(), "tester", "corr-1", CancellationToken.None);

        Assert.Equal("Estrategia Editorial Inicial", result.Name);
    }

    [Fact]
    public async Task CreateStrategyRejectsInvalidAges()
    {
        var service = new EditorialStrategyService(new FakeEditorialStrategyRepository());
        var request = ValidCreate() with { AgeFrom = 7, AgeTo = 2 };

        await Assert.ThrowsAsync<EditorialStrategyValidationException>(() => service.CreateAsync(Guid.NewGuid(), request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task CreateStrategyRejectsInvalidDurations()
    {
        var service = new EditorialStrategyService(new FakeEditorialStrategyRepository());
        var request = ValidCreate() with { MinimumEpisodeDurationSeconds = 200, DefaultEpisodeDurationSeconds = 90 };

        await Assert.ThrowsAsync<EditorialStrategyValidationException>(() => service.CreateAsync(Guid.NewGuid(), request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task CreateStrategyRejectsInvalidScenes()
    {
        var service = new EditorialStrategyService(new FakeEditorialStrategyRepository());
        var request = ValidCreate() with { ScenesMin = 11, ScenesMax = 5 };

        await Assert.ThrowsAsync<EditorialStrategyValidationException>(() => service.CreateAsync(Guid.NewGuid(), request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task ActivateDeactivatesOtherStrategies()
    {
        var repository = new FakeEditorialStrategyRepository();
        var existing = ExistingStrategy(repository.Channel.Id);
        existing.IsActive = true;
        existing.Channel = repository.Channel;
        repository.Strategies.Add(existing);
        var candidate = ExistingStrategy(repository.Channel.Id);
        candidate.IsActive = false;
        candidate.Channel = repository.Channel;
        repository.Strategies.Add(candidate);
        repository.Channel.EditorialStrategies.Add(existing);
        repository.Channel.EditorialStrategies.Add(candidate);
        var service = new EditorialStrategyService(repository);

        var activated = await service.ActivateAsync(candidate.Id, "tester", "corr-1", CancellationToken.None);

        Assert.True(activated.IsActive);
        Assert.False(existing.IsActive);
    }

    [Fact]
    public async Task ActiveStrategyCannotBeCreatedForInactiveChannel()
    {
        var repository = new FakeEditorialStrategyRepository();
        repository.Channel.IsActive = false;
        var service = new EditorialStrategyService(repository);

        await Assert.ThrowsAsync<EditorialStrategyInactiveChannelException>(() =>
            service.CreateAsync(repository.Channel.Id, ValidCreate(), "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task PillarWeightsCannotExceed100()
    {
        var repository = new FakeEditorialStrategyRepository();
        var strategy = ExistingStrategy(repository.Channel.Id);
        repository.Strategies.Add(strategy);
        var service = new EditorialStrategyService(repository);

        var request = new UpdateEditorialPillarsRequest(
            [
                new EditorialPillarDto(Guid.Empty, "a", "A", "", 80, true, 1),
                new EditorialPillarDto(Guid.Empty, "b", "B", "", 30, true, 2)
            ],
            RowVersion());

        await Assert.ThrowsAsync<EditorialStrategyValidationException>(() =>
            service.UpdatePillarsAsync(strategy.Id, request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task DuplicateTopicCodesAreRejected()
    {
        var repository = new FakeEditorialStrategyRepository();
        var strategy = ExistingStrategy(repository.Channel.Id);
        repository.Strategies.Add(strategy);
        var service = new EditorialStrategyService(repository);
        var request = new UpdateEditorialTopicsRequest(
            [
                new EditorialTopicDto(Guid.Empty, "colores", "Colores", "", "general", 1, 2, 6, true, true),
                new EditorialTopicDto(Guid.Empty, "colores", "Colores 2", "", "general", 2, 2, 6, true, true)
            ],
            RowVersion());

        await Assert.ThrowsAsync<EditorialStrategyValidationException>(() =>
            service.UpdateTopicsAsync(strategy.Id, request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task InvalidEffectiveDatesAreRejected()
    {
        var service = new EditorialStrategyService(new FakeEditorialStrategyRepository());
        var from = DateTime.UtcNow;
        var request = ValidCreate() with { EffectiveFromUtc = from, EffectiveToUtc = from.AddMinutes(-1) };

        await Assert.ThrowsAsync<EditorialStrategyValidationException>(() =>
            service.CreateAsync(Guid.NewGuid(), request, "tester", "corr-1", CancellationToken.None));
    }

    [Fact]
    public async Task BlockingRestrictionIsAccepted()
    {
        var repository = new FakeEditorialStrategyRepository();
        var strategy = ExistingStrategy(repository.Channel.Id);
        repository.Strategies.Add(strategy);
        var service = new EditorialStrategyService(repository);
        var request = new UpdateEditorialRestrictionsRequest(
            [new EditorialRestrictionDto(Guid.Empty, "SafetyRule", "armas", "Sin armas", "block", true, true)],
            RowVersion());

        var result = await service.UpdateRestrictionsAsync(strategy.Id, request, "tester", "corr-1", CancellationToken.None);

        Assert.True(result.Single().IsBlocking);
    }

    [Fact]
    public async Task ConcurrencyConflictIsPropagated()
    {
        var repository = new FakeEditorialStrategyRepository { ThrowConcurrency = true };
        var strategy = ExistingStrategy(repository.Channel.Id);
        repository.Strategies.Add(strategy);
        var service = new EditorialStrategyService(repository);

        await Assert.ThrowsAsync<EditorialStrategyConcurrencyException>(() =>
            service.UpdateAsync(strategy.Id, ValidUpdate(), "tester", "corr-1", CancellationToken.None));
    }

    private static CreateEditorialStrategyRequest ValidCreate() =>
        new("Estrategia Editorial Inicial", "", "Crear historias seguras.", "Ninos y familias", 2, 6, "es", "", "EC", "calido", "historias", "aventuras", "inicio-reto-cierre", 90, 60, 180, 5, 10, true, DateTime.UtcNow.AddDays(-1), null);

    private static UpdateEditorialStrategyRequest ValidUpdate() =>
        new("Estrategia Editorial Inicial", "", "Crear historias seguras.", "Ninos y familias", 2, 6, "es", "", "EC", "calido", "historias", "aventuras", "inicio-reto-cierre", 90, 60, 180, 5, 10, DateTime.UtcNow.AddDays(-1), null, RowVersion());

    private static EditorialStrategy ExistingStrategy(Guid channelId) =>
        new()
        {
            ChannelId = channelId,
            Name = "Estrategia",
            Objective = "Objetivo",
            PrimaryAudience = "Audiencia",
            AgeFrom = 2,
            AgeTo = 6,
            MinimumEpisodeDurationSeconds = 60,
            DefaultEpisodeDurationSeconds = 90,
            MaximumEpisodeDurationSeconds = 180,
            ScenesMin = 5,
            ScenesMax = 10,
            EffectiveFromUtc = DateTime.UtcNow.AddDays(-1),
            RowVersion = [1, 2, 3]
        };

    private static string RowVersion() => Convert.ToBase64String([1, 2, 3]);

    private sealed class FakeEditorialStrategyRepository : IEditorialStrategyRepository
    {
        public Channel Channel { get; } = new() { Name = "Historias", Code = "historias-paolin", IsActive = true };
        public List<EditorialStrategy> Strategies { get; } = [];
        public bool ThrowConcurrency { get; init; }

        public Task<Channel?> GetChannelAsync(Guid channelId, CancellationToken cancellationToken) => Task.FromResult<Channel?>(Channel);
        public Task<IReadOnlyList<EditorialStrategy>> ListByChannelAsync(Guid channelId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<EditorialStrategy>>(Strategies);
        public Task<EditorialStrategy?> GetAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Strategies.SingleOrDefault(x => x.Id == id));
        public Task<EditorialStrategy?> GetActiveAsync(Guid channelId, DateTime atUtc, CancellationToken cancellationToken) => Task.FromResult(Strategies.FirstOrDefault(x => x.IsActive));
        public Task AddAsync(EditorialStrategy strategy, CancellationToken cancellationToken) { strategy.RowVersion = [1, 2, 3]; strategy.Channel = Channel; Channel.EditorialStrategies.Add(strategy); Strategies.Add(strategy); return Task.CompletedTask; }
        public void AddOutboxMessage(OutboxMessage message) { }
        public Task SaveChangesAsync(CancellationToken cancellationToken) => ThrowConcurrency ? throw new EditorialStrategyConcurrencyException() : Task.CompletedTask;
        public void SetOriginalRowVersion(EditorialStrategy strategy, byte[] rowVersion) { }
    }
}
