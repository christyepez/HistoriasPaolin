using System.ComponentModel.DataAnnotations;

namespace HistoriasPaolin.Infrastructure.Configuration;

public sealed class SqlServerOptions
{
    public const string SectionName = "SqlServer";

    [Required]
    public string Host { get; set; } = "host.docker.internal";

    public int? Port { get; set; } = 1433;
    public string? Instance { get; set; }

    [Required]
    public string Database { get; set; } = "HistoriasPaolinDb";

    public string? User { get; set; }
    public string? Password { get; set; }
    public bool IntegratedSecurity { get; set; }
    public bool Encrypt { get; set; } = true;
    public bool TrustServerCertificate { get; set; } = true;
    public int ConnectTimeout { get; set; } = 30;
    public int CommandTimeout { get; set; } = 60;
}
