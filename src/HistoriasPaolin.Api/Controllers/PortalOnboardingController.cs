using HistoriasPaolin.Application.Portal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HistoriasPaolin.Api.Controllers;

[ApiController]
[Route("api/historiaspaolin/portal-onboarding")]
public sealed class PortalOnboardingController(IPortalOnboardingClient onboardingClient) : ControllerBase
{
    [HttpPost("register")]
    [Authorize(Policy = "historiaspaolin.settings.configure")]
    public async Task<IActionResult> Register(CancellationToken cancellationToken)
    {
        await onboardingClient.RegisterResourcesAndPermissionsAsync(cancellationToken);
        await onboardingClient.RegisterMenuAsync(cancellationToken);
        await onboardingClient.RegisterConfigurationAsync(cancellationToken);
        return Accepted(new { module = "HistoriasPaolin", status = "registration-requested" });
    }
}
