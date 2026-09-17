using System.Net.Http.Json;
using HistoriasPaolin.Application.Portal;

namespace HistoriasPaolin.Infrastructure.Portal;

public sealed class PortalAuditClient(HttpClient httpClient) : IPortalAuditClient
{
    public async Task SendAsync(PortalAuditEvent auditEvent, CancellationToken cancellationToken)
    {
        var request = new
        {
            auditEvent.ActorId,
            TenantId = "default",
            auditEvent.Resource,
            auditEvent.Action,
            auditEvent.EntityName,
            auditEvent.EntityId,
            auditEvent.BeforeJson,
            auditEvent.AfterJson,
            auditEvent.MetadataJson,
            CorrelationId = (string?)null,
            CausationId = (string?)null,
            RequestId = (string?)null,
            IpAddress = (string?)null,
            UserAgent = (string?)null,
            Severity = 1
        };

        using var response = await httpClient.PostAsJsonAsync("/api/audit/events/", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
