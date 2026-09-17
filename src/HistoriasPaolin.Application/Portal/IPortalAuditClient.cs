namespace HistoriasPaolin.Application.Portal;

public interface IPortalAuditClient
{
    Task SendAsync(PortalAuditEvent auditEvent, CancellationToken cancellationToken);
}

public sealed record PortalAuditEvent(
    string ActorId,
    string Resource,
    string Action,
    string EntityName,
    string? EntityId,
    string? BeforeJson,
    string? AfterJson,
    string? MetadataJson);
