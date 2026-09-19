namespace HistoriasPaolin.Application.Portal;

public interface IPortalOnboardingClient
{
    Task RegisterResourcesAndPermissionsAsync(CancellationToken cancellationToken);
    Task RegisterMenuAsync(CancellationToken cancellationToken);
    Task RegisterConfigurationAsync(CancellationToken cancellationToken);
}
