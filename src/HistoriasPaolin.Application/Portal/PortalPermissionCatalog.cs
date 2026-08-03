namespace HistoriasPaolin.Application.Portal;

public static class PortalPermissionCatalog
{
    public static readonly string[] Roles =
    [
        "HistoriasPaolin.Admin",
        "HistoriasPaolin.Producer",
        "HistoriasPaolin.Reviewer",
        "HistoriasPaolin.Publisher",
        "HistoriasPaolin.Viewer"
    ];

    public static readonly string[] Resources =
    [
        "historiaspaolin.dashboard",
        "historiaspaolin.episodes",
        "historiaspaolin.scenes",
        "historiaspaolin.characters",
        "historiaspaolin.prompts",
        "historiaspaolin.media",
        "historiaspaolin.quality",
        "historiaspaolin.publications",
        "historiaspaolin.settings",
        "historiaspaolin.costs",
        "historiaspaolin.operations"
    ];

    public static readonly string[] Actions =
    [
        "view",
        "create",
        "update",
        "delete",
        "generate",
        "approve",
        "reject",
        "regenerate",
        "upload",
        "schedule",
        "publish",
        "cancel",
        "retry",
        "configure",
        "view_costs",
        "view_logs"
    ];

    public static string Permission(string resource, string action) => $"{resource}.{action}";
}
