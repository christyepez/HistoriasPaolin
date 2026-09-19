namespace HistoriasPaolin.Worker;

public class Worker(ILogger<Worker> logger, IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("HistoriasPaolin worker heartbeat at {Time}", DateTimeOffset.UtcNow);
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
