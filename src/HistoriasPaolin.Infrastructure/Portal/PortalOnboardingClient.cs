using System.Net;
using System.Net.Http.Json;
using HistoriasPaolin.Application.Portal;
using Microsoft.Extensions.Options;

namespace HistoriasPaolin.Infrastructure.Portal;

public sealed class PortalOnboardingClient(HttpClient httpClient, IOptions<PortalOptions> options) : IPortalOnboardingClient
{
    private readonly PortalOptions _options = options.Value;

    public async Task RegisterResourcesAndPermissionsAsync(CancellationToken cancellationToken)
    {
        foreach (var resource in PortalPermissionCatalog.Resources)
        {
            await PostIdempotentAsync("/api/security/resources", new { Key = resource, Name = resource }, cancellationToken);
            foreach (var action in PortalPermissionCatalog.Actions)
            {
                await PostIdempotentAsync("/api/security/permissions", new
                {
                    Code = PortalPermissionCatalog.Permission(resource, action),
                    ResourceKey = resource,
                    Action = action
                }, cancellationToken);
            }
        }

        foreach (var role in PortalPermissionCatalog.Roles)
        {
            await PostIdempotentAsync("/api/security/roles", new { Name = role }, cancellationToken);
        }
    }

    public async Task RegisterMenuAsync(CancellationToken cancellationToken)
    {
        await PostIdempotentAsync("/api/menu/", new { ModuleCode = _options.ModuleCode, Name = _options.ModuleName }, cancellationToken);
    }

    public async Task RegisterConfigurationAsync(CancellationToken cancellationToken)
    {
        await PostIdempotentAsync("/api/configuration/items", new
        {
            Key = "historiaspaolin.module.settings",
            Scope = 2,
            ModuleCode = _options.ModuleCode,
            UserId = (Guid?)null,
            Category = 1,
            ValueJson = "{\"defaultLanguage\":\"es-EC\",\"autoPublishEnabled\":false,\"madeForKids\":true}"
        }, cancellationToken);
    }

    private async Task PostIdempotentAsync(string path, object body, CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(path, body, cancellationToken);
        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Conflict)
        {
            return;
        }

        response.EnsureSuccessStatusCode();
    }
}
