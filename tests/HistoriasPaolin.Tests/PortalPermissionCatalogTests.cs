using HistoriasPaolin.Application.Portal;

namespace HistoriasPaolin.Tests;

public sealed class PortalPermissionCatalogTests
{
    [Fact]
    public void ViewerReviewerAndProducerDoNotReceivePublishByPolicyDocumentation()
    {
        var restrictedRoles = new[]
        {
            "HistoriasPaolin.Viewer",
            "HistoriasPaolin.Reviewer",
            "HistoriasPaolin.Producer"
        };

        Assert.All(restrictedRoles, role => Assert.Contains(role, PortalPermissionCatalog.Roles));
        Assert.Contains("publish", PortalPermissionCatalog.Actions);
    }

    [Fact]
    public void PermissionCodeCombinesResourceAndAction()
    {
        var permission = PortalPermissionCatalog.Permission("historiaspaolin.publications", "publish");
        Assert.Equal("historiaspaolin.publications.publish", permission);
    }
}
