using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HistoriasPaolin.Infrastructure.Health;

public sealed class DirectoryHealthCheck(string path) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            Directory.CreateDirectory(path);
            var probe = Path.Combine(path, $".healthcheck-{Guid.NewGuid():N}");
            File.WriteAllText(probe, DateTimeOffset.UtcNow.ToString("O"));
            File.Delete(probe);
            return Task.FromResult(HealthCheckResult.Healthy($"Directory is writable: {path}"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Directory is not writable: {path}", ex));
        }
    }
}
