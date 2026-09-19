using Microsoft.Data.SqlClient;

namespace HistoriasPaolin.Infrastructure.Configuration;

public static class SqlServerConnectionStringFactory
{
    public static string Create(SqlServerOptions options)
    {
        Validate(options);

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = BuildDataSource(options),
            InitialCatalog = options.Database,
            Encrypt = options.Encrypt,
            TrustServerCertificate = options.TrustServerCertificate,
            ConnectTimeout = options.ConnectTimeout,
            MultipleActiveResultSets = true,
            Pooling = true
        };

        if (options.IntegratedSecurity)
        {
            builder.IntegratedSecurity = true;
        }
        else
        {
            builder.UserID = options.User;
            builder.Password = options.Password;
        }

        return builder.ConnectionString;
    }

    public static string BuildDataSource(SqlServerOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Instance))
        {
            var instance = options.Instance.Trim();
            if (!instance.StartsWith('\\'))
            {
                instance = "\\" + instance;
            }

            return $"{options.Host}{instance}";
        }

        return options.Port is > 0 ? $"{options.Host},{options.Port}" : options.Host;
    }

    public static void Validate(SqlServerOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Host))
        {
            throw new InvalidOperationException("SQLSERVER_HOST is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Database))
        {
            throw new InvalidOperationException("SQLSERVER_DATABASE is required.");
        }

        if (!options.IntegratedSecurity &&
            (string.IsNullOrWhiteSpace(options.User) || string.IsNullOrWhiteSpace(options.Password)))
        {
            throw new InvalidOperationException("SQLSERVER_USER and SQLSERVER_PASSWORD are required when SQLSERVER_INTEGRATED_SECURITY=false.");
        }

        if (options.CommandTimeout <= 0 || options.ConnectTimeout <= 0)
        {
            throw new InvalidOperationException("SQL Server timeouts must be positive integers.");
        }
    }
}
