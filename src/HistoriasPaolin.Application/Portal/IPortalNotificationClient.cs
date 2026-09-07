namespace HistoriasPaolin.Application.Portal;

public interface IPortalNotificationClient
{
    Task SendAsync(PortalNotificationRequest request, CancellationToken cancellationToken);
}

public sealed record PortalNotificationRequest(
    string TemplateCode,
    string[] Recipients,
    Dictionary<string, string>? Variables,
    string IdempotencyKey,
    string? MetadataJson);
