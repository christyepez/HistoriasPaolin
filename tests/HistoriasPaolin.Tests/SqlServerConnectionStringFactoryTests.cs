using HistoriasPaolin.Infrastructure.Configuration;

namespace HistoriasPaolin.Tests;

public sealed class SqlServerConnectionStringFactoryTests
{
    [Fact]
    public void UsesPortWhenNoNamedInstanceIsConfigured()
    {
        var options = ValidOptions();
        options.Port = 1433;
        var connectionString = SqlServerConnectionStringFactory.Create(options);

        Assert.Contains("Data Source=host.docker.internal,1433", connectionString);
        Assert.DoesNotContain("\\SQLEXPRESS", connectionString);
    }

    [Fact]
    public void UsesNamedInstanceWithoutPort()
    {
        var options = ValidOptions();
        options.Instance = "SQLEXPRESS";

        var connectionString = SqlServerConnectionStringFactory.Create(options);

        Assert.Contains(@"Data Source=host.docker.internal\SQLEXPRESS", connectionString);
        Assert.DoesNotContain(",1433", connectionString);
    }

    [Fact]
    public void RejectsEmptySqlCredentialsWhenIntegratedSecurityIsDisabled()
    {
        var options = ValidOptions();
        options.User = "";

        var ex = Assert.Throws<InvalidOperationException>(() => SqlServerConnectionStringFactory.Create(options));
        Assert.Contains("SQLSERVER_USER", ex.Message);
    }

    [Fact]
    public void AllowsIntegratedSecurityWithoutPassword()
    {
        var options = ValidOptions();
        options.IntegratedSecurity = true;
        options.User = "";
        options.Password = "";

        var connectionString = SqlServerConnectionStringFactory.Create(options);

        Assert.Contains("Integrated Security=True", connectionString);
    }

    private static SqlServerOptions ValidOptions() => new()
    {
        Host = "host.docker.internal",
        Port = 1433,
        Database = "HistoriasPaolinDb",
        User = "app",
        Password = "local-secret",
        IntegratedSecurity = false,
        Encrypt = true,
        TrustServerCertificate = true,
        ConnectTimeout = 30,
        CommandTimeout = 60
    };
}
