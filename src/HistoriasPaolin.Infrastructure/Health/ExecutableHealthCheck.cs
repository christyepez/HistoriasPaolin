using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HistoriasPaolin.Infrastructure.Health;

public sealed class ExecutableHealthCheck(string executableName) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var extensions = OperatingSystem.IsWindows()
            ? (Environment.GetEnvironmentVariable("PATHEXT") ?? ".EXE;.BAT;.CMD").Split(';')
            : [string.Empty];

        foreach (var directory in pathVariable.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var extension in extensions)
            {
                var candidate = Path.Combine(directory, executableName + extension);
                if (File.Exists(candidate))
                {
                    return Task.FromResult(HealthCheckResult.Healthy($"{executableName} found."));
                }
            }
        }

        return Task.FromResult(HealthCheckResult.Degraded($"{executableName} was not found in PATH."));
    }
}
