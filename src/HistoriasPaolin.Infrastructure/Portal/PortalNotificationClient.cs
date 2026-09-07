using System.Net.Http.Json;
using HistoriasPaolin.Application.Portal;

namespace HistoriasPaolin.Infrastructure.Portal;

public sealed class PortalNotificationClient(HttpClient httpClient) : IPortalNotificationClient
{
    public async Task SendAsync(PortalNotificationRequest request, CancellationToken cancellationToken)
    {
        var body = new
        {
            request.TemplateCode,
            request.Recipients,
            request.Variables,
            Channel = (int?)null,
            request.IdempotencyKey,
            request.MetadataJson
        };

        using var response = await httpClient.PostAsJsonAsync("/api/notifications/send", body, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
