using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

    [Fact]
    public void OidcAuthorityModeConfiguresBearerOptions()
    {
        using var oidcFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Jwt:Authority", "https://idp.corp.internal/");
            builder.UseSetting("Jwt:Audience", "portal-api");
            builder.UseSetting("Jwt:RequireHttpsMetadata", "true");
        });

        using var scope = oidcFactory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Equal("https://idp.corp.internal/", options.Authority);
        Assert.Equal("portal-api", options.Audience);
        Assert.True(options.RequireHttpsMetadata);
        Assert.Null(options.TokenValidationParameters.IssuerSigningKey);
    }
}
