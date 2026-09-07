using HistoriasPaolin.Application.Common;
using HistoriasPaolin.Application.Channels;
using HistoriasPaolin.Application.Episodes;
using HistoriasPaolin.Application.EditorialStrategies;
using HistoriasPaolin.Application.Portal;
using HistoriasPaolin.Infrastructure.Configuration;
using HistoriasPaolin.Infrastructure.Health;
using HistoriasPaolin.Infrastructure.Persistence;
using HistoriasPaolin.Infrastructure.Portal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace HistoriasPaolin.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddHistoriasPaolinInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICurrentUser, SystemCurrentUser>();
        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationIdHandler>();
        services.AddScoped<AuditingSaveChangesInterceptor>();
        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<IChannelService, ChannelService>();
        services.AddScoped<IEditorialStrategyRepository, EditorialStrategyRepository>();
        services.AddScoped<IEditorialStrategyService, EditorialStrategyService>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();

        services.AddOptions<SqlServerOptions>()
            .Bind(configuration.GetSection(SqlServerOptions.SectionName))
            .Configure(options => ApplySqlServerEnvironmentFallback(options))
            .ValidateDataAnnotations()
            .Validate(options =>
            {
                try
                {
                    SqlServerConnectionStringFactory.Validate(options);
                    return true;
                }
                catch
                {
                    return false;
                }
            }, "SQL Server configuration is invalid.")
            .ValidateOnStart();

        services.AddOptions<PortalOptions>()
            .Bind(configuration.GetSection(PortalOptions.SectionName))
            .Configure(options =>
            {
                options.GatewayBaseUrl = Environment.GetEnvironmentVariable("PORTAL_GATEWAY_BASE_URL") ?? options.GatewayBaseUrl;
            })
            .Validate(options => Uri.TryCreate(options.GatewayBaseUrl, UriKind.Absolute, out _), "Portal GatewayBaseUrl must be absolute.")
            .ValidateOnStart();

        services.AddPortalHttpClient<IPortalAuditClient, PortalAuditClient>(configuration);
        services.AddPortalHttpClient<IPortalNotificationClient, PortalNotificationClient>(configuration);
        services.AddPortalHttpClient<IPortalOnboardingClient, PortalOnboardingClient>(configuration);

        services.AddDbContext<HistoriasPaolinDbContext>((serviceProvider, options) =>
        {
            var sqlOptions = serviceProvider.GetRequiredService<IOptions<SqlServerOptions>>().Value;
            var connectionString = SqlServerConnectionStringFactory.Create(sqlOptions);
            var auditInterceptor = serviceProvider.GetRequiredService<AuditingSaveChangesInterceptor>();

            options.UseSqlServer(connectionString, sql =>
                {
                    sql.CommandTimeout(sqlOptions.CommandTimeout);
                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    sql.MigrationsAssembly(typeof(HistoriasPaolinDbContext).Assembly.FullName);
                })
                .AddInterceptors(auditInterceptor);
        });

        services.AddHealthChecks()
            .AddCheck<SqlServerDbContextHealthCheck>("sqlserver", HealthStatus.Unhealthy, ["ready", "sqlserver"])
            .AddCheck("storage", new DirectoryHealthCheck(Path.Combine(AppContext.BaseDirectory, "production")), failureStatus: HealthStatus.Unhealthy, tags: ["ready", "storage"])
            .AddCheck("ffmpeg", new ExecutableHealthCheck("ffmpeg"), failureStatus: HealthStatus.Degraded, tags: ["live", "ffmpeg"])
            .AddCheck("higgsfield-cli", new ExecutableHealthCheck("higgsfield"), failureStatus: HealthStatus.Degraded, tags: ["live", "higgsfield"])
            .AddCheck("worker", () => HealthCheckResult.Healthy("Worker process is running."), tags: ["live", "worker"]);

        return services;
    }

    private static IServiceCollection AddPortalHttpClient<TClient, TImplementation>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TClient : class
        where TImplementation : class, TClient
    {
        var gatewayBaseUrl = configuration.GetSection(PortalOptions.SectionName).GetValue<string>(nameof(PortalOptions.GatewayBaseUrl))
            ?? Environment.GetEnvironmentVariable("PORTAL_GATEWAY_BASE_URL")
            ?? "http://api-gateway:8080";

        services.AddHttpClient<TClient, TImplementation>(client =>
            {
                client.BaseAddress = new Uri(gatewayBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<CorrelationIdHandler>();

        return services;
    }

    private static void ApplySqlServerEnvironmentFallback(SqlServerOptions options)
    {
        options.Host = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? options.Host;
        options.Instance = Environment.GetEnvironmentVariable("SQLSERVER_INSTANCE") ?? options.Instance;
        options.Database = Environment.GetEnvironmentVariable("SQLSERVER_DATABASE") ?? options.Database;
        options.User = Environment.GetEnvironmentVariable("SQLSERVER_USER") ?? options.User;
        options.Password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD") ?? options.Password;

        if (int.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_PORT"), out var port))
        {
            options.Port = port;
        }

        if (bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_INTEGRATED_SECURITY"), out var integratedSecurity))
        {
            options.IntegratedSecurity = integratedSecurity;
        }

        if (bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_ENCRYPT"), out var encrypt))
        {
            options.Encrypt = encrypt;
        }

        if (bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_TRUST_SERVER_CERTIFICATE"), out var trustServerCertificate))
        {
            options.TrustServerCertificate = trustServerCertificate;
        }

        if (int.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_CONNECT_TIMEOUT"), out var connectTimeout))
        {
            options.ConnectTimeout = connectTimeout;
        }

        if (int.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_COMMAND_TIMEOUT"), out var commandTimeout))
        {
            options.CommandTimeout = commandTimeout;
        }
    }
}
