using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using HistoriasPaolin.Application.Channels;
using HistoriasPaolin.Contracts.Channels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace HistoriasPaolin.IntegrationTests;

public sealed class ChannelEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ChannelEndpointTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("SQLSERVER_INTEGRATED_SECURITY", "true");
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IChannelService>();
                services.AddSingleton<IChannelService, FakeChannelService>();
            });
        });
    }

    [Fact]
    public async Task GetChannelsRequiresViewPermission()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.view");

        using var response = await client.GetAsync("/api/channels");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostChannelRequiresManagePermission()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.view");

        using var response = await client.PostAsync("/api/channels", JsonContent(ValidCreateRequest()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostChannelCreatesChannel()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.manage");

        using var response = await client.PostAsync("/api/channels", JsonContent(ValidCreateRequest()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetByIdReturnsChannel()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.view");

        using var response = await client.GetAsync($"/api/channels/{FakeChannelService.ChannelId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByIdReturnsAuditMetadata()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.view");

        using var response = await client.GetAsync($"/api/channels/{FakeChannelService.ChannelId}");
        var channel = await response.Content.ReadFromJsonAsync<ChannelDetailDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(channel);
        Assert.Equal("tests", channel!.Audit.CreatedBy);
        Assert.Equal("tests", channel.Audit.UpdatedBy);
        Assert.NotEqual(default, channel.Audit.CreatedAtUtc);
        Assert.NotNull(channel.Audit.UpdatedAtUtc);
    }

    [Fact]
    public async Task GetByCodeReturnsChannel()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.view");

        using var response = await client.GetAsync("/api/channels/by-code/historias-paolin");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PutChannelUpdatesChannel()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.manage");

        using var response = await client.PutAsync($"/api/channels/{FakeChannelService.ChannelId}", JsonContent(ValidUpdateRequest()));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PatchStatusUpdatesChannel()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.manage");

        using var response = await client.PatchAsync($"/api/channels/{FakeChannelService.ChannelId}/status", JsonContent(new UpdateChannelStatusRequest(false, RowVersion())));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PutBrandUpdatesBrand()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.manage");

        using var response = await client.PutAsync($"/api/channels/{FakeChannelService.ChannelId}/brand", JsonContent(ValidBrandRequest()));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBrandReturnsAuditMetadata()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.view");

        using var response = await client.GetAsync($"/api/channels/{FakeChannelService.ChannelId}/brand");
        var brand = await response.Content.ReadFromJsonAsync<ChannelBrandDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(brand);
        Assert.Equal("tests", brand!.Audit.CreatedBy);
        Assert.Equal("tests", brand.Audit.UpdatedBy);
    }

    [Fact]
    public async Task WriteWithoutTokenIsUnauthorized()
    {
        using var client = _factory.CreateClient();

        using var response = await client.PostAsync("/api/channels", JsonContent(ValidCreateRequest()));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ConcurrencyConflictReturns409()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.manage");

        using var response = await client.PutAsync($"/api/channels/{FakeChannelService.ConcurrencyId}", JsonContent(ValidUpdateRequest()));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetMissingChannelReturns404()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.view");

        using var response = await client.GetAsync($"/api/channels/{FakeChannelService.NotFoundId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrorReturns400()
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = Bearer("historiaspaolin.channels.manage");

        using var response = await client.PostAsync("/api/channels", JsonContent(ValidCreateRequest() with { Code = "invalid" }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static AuthenticationHeaderValue Bearer(string permission) => new("Bearer", CreateToken(permission));

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

    private static StringContent JsonContent<T>(T value) =>
        new(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

    private static string RowVersion() => Convert.ToBase64String([1, 2, 3]);

    private static CreateChannelRequest ValidCreateRequest() =>
        new("historias-paolin", "Historias de Paolin", "Canal infantil", "es", "EC", "America/Guayaquil", true, "16:9", 180, "private");

    private static UpdateChannelRequest ValidUpdateRequest() =>
        new("Historias de Paolin", "Canal infantil", "es", "EC", "America/Guayaquil", true, "16:9", 180, "private", RowVersion());

    private static UpdateChannelBrandRequest ValidBrandRequest() =>
        new("Historias de Paolin", "Historias", "Historias educativas", "es", "Colorido", "Calido", "Familias", 2, 6, "Prompt", "Consistencia", "Negativo", true, null);

    private sealed class FakeChannelService : IChannelService
    {
        public static readonly Guid ChannelId = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa");
        public static readonly Guid ConcurrencyId = Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb");
        public static readonly Guid NotFoundId = Guid.Parse("dddddddd-dddd-4ddd-8ddd-dddddddddddd");

        public Task<IReadOnlyList<ChannelSummaryDto>> ListAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<ChannelSummaryDto>>([Summary()]);

        public Task<ChannelDetailDto> GetAsync(Guid id, CancellationToken cancellationToken) =>
            id == NotFoundId ? throw new ChannelNotFoundException(id) :
            id == ConcurrencyId ? throw new ChannelConcurrencyException() :
            Task.FromResult(Detail(id));

        public Task<ChannelDetailDto> GetByCodeAsync(string code, CancellationToken cancellationToken) => Task.FromResult(Detail(ChannelId));

        public Task<ChannelDetailDto> CreateAsync(CreateChannelRequest request, string actor, string correlationId, CancellationToken cancellationToken) =>
            request.Code == "invalid"
                ? throw new ChannelValidationException(["Code is required."])
                : Task.FromResult(Detail(ChannelId));

        public Task<ChannelDetailDto> UpdateAsync(Guid id, UpdateChannelRequest request, string actor, string correlationId, CancellationToken cancellationToken) =>
            id == ConcurrencyId ? throw new ChannelConcurrencyException() : Task.FromResult(Detail(id));

        public Task<ChannelDetailDto> UpdateStatusAsync(Guid id, UpdateChannelStatusRequest request, string actor, string correlationId, CancellationToken cancellationToken) =>
            Task.FromResult(Detail(id) with { IsActive = request.IsActive });

        public Task<ChannelBrandDto> GetBrandAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Brand(id));

        public Task<ChannelBrandDto> UpdateBrandAsync(Guid id, UpdateChannelBrandRequest request, string actor, string correlationId, CancellationToken cancellationToken) =>
            Task.FromResult(Brand(id));

        private static ChannelSummaryDto Summary() => new(ChannelId, "historias-paolin", "Historias de Paolin", "es", "EC", true, true, "private");

        private static ChannelDetailDto Detail(Guid id) =>
            new(id, "historias-paolin", "Historias de Paolin", "Canal infantil", "es", "EC", "America/Guayaquil", true, true, "16:9", 180, "private", RowVersion(), Audit(), Brand(id));

        private static ChannelBrandDto Brand(Guid id) =>
            new(Guid.Parse("cccccccc-cccc-4ccc-8ccc-cccccccccccc"), id, "Historias de Paolin", "Historias", "Historias educativas", "es", "Colorido", "Calido", "Familias", 2, 6, "Prompt", "Consistencia", "Negativo", true, RowVersion(), Audit());

        private static AuditMetadataDto Audit() => new(DateTime.UtcNow.AddDays(-1), "tests", DateTime.UtcNow, "tests");
    }
}
