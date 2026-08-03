namespace HistoriasPaolin.Application.Portal;

public sealed class PortalOptions
{
    public const string SectionName = "Portal";
    public string GatewayBaseUrl { get; set; } = "http://api-gateway:8080";
    public string ModuleCode { get; set; } = "HistoriasPaolin";
    public string ModuleName { get; set; } = "Historias de Paolin";
}
