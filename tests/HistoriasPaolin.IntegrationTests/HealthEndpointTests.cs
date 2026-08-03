using Microsoft.AspNetCore.Mvc.Testing;

namespace HistoriasPaolin.IntegrationTests;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("SQLSERVER_INTEGRATED_SECURITY", "true");
        _factory = factory;
    }

    [Fact]
    public async Task LiveHealthEndpointDoesNotRequireSqlServer()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/health/live");

        Assert.True((int)response.StatusCode < 500);
    }
}
