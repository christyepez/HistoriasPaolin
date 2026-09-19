using System.Text;
using HistoriasPaolin.Api.Middleware;
using HistoriasPaolin.Api.Security;
using HistoriasPaolin.Application.Portal;
using HistoriasPaolin.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHistoriasPaolinInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

var jwtAuthority = builder.Configuration["Jwt:Authority"] ?? builder.Configuration["JWT_AUTHORITY"];
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? builder.Configuration["JWT_AUDIENCE"]
    ?? "portal-corporativo-clients";
var requireHttpsMetadataValue = builder.Configuration["Jwt:RequireHttpsMetadata"]
    ?? builder.Configuration["JWT_REQUIRE_HTTPS_METADATA"];
var requireHttpsMetadata = !bool.TryParse(requireHttpsMetadataValue, out var parsedRequireHttpsMetadata)
    || parsedRequireHttpsMetadata;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (!string.IsNullOrWhiteSpace(jwtAuthority))
        {
            options.Authority = jwtAuthority.Trim();
            options.Audience = jwtAudience;
            options.RequireHttpsMetadata = requireHttpsMetadata;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
            return;
        }

        var jwtSecret = builder.Configuration["Jwt:Secret"]
            ?? builder.Configuration["JWT_SECRET"]
            ?? (builder.Environment.IsDevelopment()
                ? "local-development-secret-change-me-32"
                : throw new InvalidOperationException("Configure Jwt:Authority/JWT_AUTHORITY for OIDC or provide Jwt:Secret/JWT_SECRET for local JWT mode."));
        var jwtIssuer = builder.Configuration["Jwt:Issuer"]
            ?? builder.Configuration["JWT_ISSUER"]
            ?? "portal-corporativo";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
{
    foreach (var resource in PortalPermissionCatalog.Resources)
    {
        foreach (var action in PortalPermissionCatalog.Actions)
        {
            var permission = PortalPermissionCatalog.Permission(resource, action);
            options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionAuthorizationRequirement(permission)));
        }
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions());
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();

public partial class Program;
