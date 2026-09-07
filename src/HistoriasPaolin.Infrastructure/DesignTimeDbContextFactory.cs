using HistoriasPaolin.Application.Common;
using HistoriasPaolin.Infrastructure.Configuration;
using HistoriasPaolin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HistoriasPaolin.Infrastructure;

public sealed class DesignTimeDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<HistoriasPaolinDbContext>
{
    public HistoriasPaolinDbContext CreateDbContext(string[] args)
    {
        var options = new SqlServerOptions
        {
            Host = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "host.docker.internal",
            Port = int.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_PORT"), out var port) ? port : 1433,
            Instance = Environment.GetEnvironmentVariable("SQLSERVER_INSTANCE"),
            Database = Environment.GetEnvironmentVariable("SQLSERVER_DATABASE") ?? "HistoriasPaolinDb",
            User = Environment.GetEnvironmentVariable("SQLSERVER_USER"),
            Password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD"),
            IntegratedSecurity = bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_INTEGRATED_SECURITY"), out var integrated) && integrated,
            Encrypt = !bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_ENCRYPT"), out var encrypt) || encrypt,
            TrustServerCertificate = !bool.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_TRUST_SERVER_CERTIFICATE"), out var trust) || trust,
            ConnectTimeout = int.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_CONNECT_TIMEOUT"), out var connectTimeout) ? connectTimeout : 30,
            CommandTimeout = int.TryParse(Environment.GetEnvironmentVariable("SQLSERVER_COMMAND_TIMEOUT"), out var commandTimeout) ? commandTimeout : 60
        };

        var builder = new DbContextOptionsBuilder<HistoriasPaolinDbContext>();
        builder.UseSqlServer(SqlServerConnectionStringFactory.Create(options), sql =>
        {
            sql.CommandTimeout(options.CommandTimeout);
            sql.EnableRetryOnFailure();
        });

        builder.AddInterceptors(new AuditingSaveChangesInterceptor(new SystemCurrentUser()));
        return new HistoriasPaolinDbContext(builder.Options);
    }
}
