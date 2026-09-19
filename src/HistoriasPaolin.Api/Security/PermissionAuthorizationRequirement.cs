using Microsoft.AspNetCore.Authorization;

namespace HistoriasPaolin.Api.Security;

public sealed record PermissionAuthorizationRequirement(string Permission) : IAuthorizationRequirement;
