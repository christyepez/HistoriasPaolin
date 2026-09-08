using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using HistoriasPaolin.Application.EditorialStrategies;
using HistoriasPaolin.Contracts.EditorialStrategies;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace HistoriasPaolin.IntegrationTests;

public sealed class EditorialStrategyEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EditorialStrategyEndpointTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("SQLSERVER_INTEGRATED_SECURITY", "true");
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IEditorialStrategyService>();
                services.AddSingleton<IEditorialStrategyService, FakeEditorialStrategyService>();
            });
        });
    }

    [Fact]
    public async Task PostStrategyCreatesStrategy()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.PostAsync($"/api/channels/{FakeEditorialStrategyService.ChannelId}/editorial-strategies", JsonContent(ValidCreate()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetStrategyReturnsStrategy()
    {
        using var client = Client("historiaspaolin.editorial.view");

        using var response = await client.GetAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetStrategyReturnsAuditMetadata()
    {
        using var client = Client("historiaspaolin.editorial.view");

        using var response = await client.GetAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}");
        var strategy = await response.Content.ReadFromJsonAsync<EditorialStrategyDetailDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(strategy);
        Assert.Equal("tests", strategy!.Audit.CreatedBy);
        Assert.Equal("tests", strategy.Audit.UpdatedBy);
        Assert.NotEqual(default, strategy.Audit.CreatedAtUtc);
        Assert.NotNull(strategy.Audit.UpdatedAtUtc);
    }

    [Fact]
    public async Task GetActiveReturnsStrategy()
    {
        using var client = Client("historiaspaolin.editorial.view");

        using var response = await client.GetAsync($"/api/channels/{FakeEditorialStrategyService.ChannelId}/editorial-strategy/active");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PutStrategyUpdatesStrategy()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.PutAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}", JsonContent(ValidUpdate()));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ActivateRequiresActivatePermission()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.PostAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}/activate", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ActivateStrategyReturnsOk()
    {
        using var client = Client("historiaspaolin.editorial.activate");

        using var response = await client.PostAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}/activate", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeactivateStrategyReturnsOk()
    {
        using var client = Client("historiaspaolin.editorial.activate");

        using var response = await client.PostAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}/deactivate", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PillarsCanBeUpdated()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.PutAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}/pillars", JsonContent(new UpdateEditorialPillarsRequest([new EditorialPillarDto(Guid.Empty, "educacion", "Educacion", "", 50, true, 1)], RowVersion())));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TopicsCanBeUpdated()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.PutAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}/topics", JsonContent(new UpdateEditorialTopicsRequest([new EditorialTopicDto(Guid.Empty, "colores", "Colores", "", "general", 1, 2, 6, true, true)], RowVersion())));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RestrictionsCanBeUpdated()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.PutAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}/restrictions", JsonContent(new UpdateEditorialRestrictionsRequest([new EditorialRestrictionDto(Guid.Empty, "SafetyRule", "armas", "Sin armas", "block", true, true)], RowVersion())));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReadRequiresViewPermission()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.GetAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task WriteRequiresManagePermission()
    {
        using var client = Client("historiaspaolin.editorial.view");

        using var response = await client.PutAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.StrategyId}", JsonContent(ValidUpdate()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ConcurrencyConflictReturns409()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.PutAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.ConcurrencyId}", JsonContent(ValidUpdate()));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetMissingStrategyReturns404()
    {
        using var client = Client("historiaspaolin.editorial.view");

        using var response = await client.GetAsync($"/api/editorial-strategies/{FakeEditorialStrategyService.NotFoundId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrorReturns400()
    {
        using var client = Client("historiaspaolin.editorial.manage");

        using var response = await client.PostAsync($"/api/channels/{FakeEditorialStrategyService.ChannelId}/editorial-strategies", JsonContent(ValidCreate() with { Name = "invalid" }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private HttpClient Client(string permission)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(permission));
        return client;
    }

    private static string CreateToken(string permission)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("local-development-secret-change-me-32"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "portal-corporativo",
            audience: "portal-corporativo-clients",
            claims: [new Claim("permission", permission), new Claim("sub", "tests")],
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static StringContent JsonContent<T>(T value) => new(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

    private static string RowVersion() => Convert.ToBase64String([1, 2, 3]);

    private static CreateEditorialStrategyRequest ValidCreate() =>
        new("Estrategia", "", "Objetivo", "Audiencia", 2, 6, "es", "", "EC", "calido", "educativo", "aventuras", "inicio-cierre", 90, 60, 180, 5, 10, true, DateTime.UtcNow.AddDays(-1), null);

    private static UpdateEditorialStrategyRequest ValidUpdate() =>
        new("Estrategia", "", "Objetivo", "Audiencia", 2, 6, "es", "", "EC", "calido", "educativo", "aventuras", "inicio-cierre", 90, 60, 180, 5, 10, DateTime.UtcNow.AddDays(-1), null, RowVersion());

    private sealed class FakeEditorialStrategyService : IEditorialStrategyService
    {
        public static readonly Guid ChannelId = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa");
        public static readonly Guid StrategyId = Guid.Parse("dddddddd-dddd-4ddd-8ddd-dddddddddddd");
        public static readonly Guid ConcurrencyId = Guid.Parse("eeeeeeee-eeee-4eee-8eee-eeeeeeeeeeee");
        public static readonly Guid NotFoundId = Guid.Parse("ffffffff-ffff-4fff-8fff-ffffffffffff");

        public Task<IReadOnlyList<EditorialStrategySummaryDto>> ListAsync(Guid channelId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<EditorialStrategySummaryDto>>([new EditorialStrategySummaryDto(StrategyId, ChannelId, "Estrategia", true, 1, DateTime.UtcNow.AddDays(-1), null)]);
        public Task<EditorialStrategyDetailDto> GetAsync(Guid id, CancellationToken cancellationToken) => id == NotFoundId ? throw new EditorialStrategyNotFoundException(id) : id == ConcurrencyId ? throw new EditorialStrategyConcurrencyException() : Task.FromResult(Detail(id));
        public Task<EditorialStrategyDetailDto> GetActiveAsync(Guid channelId, CancellationToken cancellationToken) => Task.FromResult(Detail(StrategyId));
        public Task<EditorialStrategyDetailDto> CreateAsync(Guid channelId, CreateEditorialStrategyRequest request, string actor, string correlationId, CancellationToken cancellationToken) =>
            request.Name == "invalid"
                ? throw new EditorialStrategyValidationException(["Name is required."])
                : Task.FromResult(Detail(StrategyId));
        public Task<EditorialStrategyDetailDto> UpdateAsync(Guid id, UpdateEditorialStrategyRequest request, string actor, string correlationId, CancellationToken cancellationToken) => id == ConcurrencyId ? throw new EditorialStrategyConcurrencyException() : Task.FromResult(Detail(id));
        public Task<EditorialStrategyDetailDto> ActivateAsync(Guid id, string actor, string correlationId, CancellationToken cancellationToken) => Task.FromResult(Detail(id) with { IsActive = true });
        public Task<EditorialStrategyDetailDto> DeactivateAsync(Guid id, string actor, string correlationId, CancellationToken cancellationToken) => Task.FromResult(Detail(id) with { IsActive = false });
        public Task<IReadOnlyList<EditorialPillarDto>> GetPillarsAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<EditorialPillarDto>>([]);
        public Task<IReadOnlyList<EditorialPillarDto>> UpdatePillarsAsync(Guid id, UpdateEditorialPillarsRequest request, string actor, string correlationId, CancellationToken cancellationToken) => Task.FromResult(request.Pillars);
        public Task<IReadOnlyList<EditorialTopicDto>> GetTopicsAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<EditorialTopicDto>>([]);
        public Task<IReadOnlyList<EditorialTopicDto>> UpdateTopicsAsync(Guid id, UpdateEditorialTopicsRequest request, string actor, string correlationId, CancellationToken cancellationToken) => Task.FromResult(request.Topics);
        public Task<IReadOnlyList<EditorialRestrictionDto>> GetRestrictionsAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<EditorialRestrictionDto>>([]);
        public Task<IReadOnlyList<EditorialRestrictionDto>> UpdateRestrictionsAsync(Guid id, UpdateEditorialRestrictionsRequest request, string actor, string correlationId, CancellationToken cancellationToken) => Task.FromResult(request.Restrictions);

        private static EditorialStrategyDetailDto Detail(Guid id) => new(id, ChannelId, "Estrategia", "", "Objetivo", "Audiencia", 2, 6, "es", "", "EC", "calido", "educativo", "aventuras", "inicio-cierre", 90, 60, 180, 5, 10, true, 1, DateTime.UtcNow.AddDays(-1), null, RowVersion(), Audit());

        private static AuditMetadataDto Audit() => new(DateTime.UtcNow.AddDays(-1), "tests", DateTime.UtcNow, "tests");
    }
}
