namespace Portfolio.Api.Services;

public class PriceRefreshService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriceRefreshService> _logger;

    public PriceRefreshService(IServiceScopeFactory scopeFactory, ILogger<PriceRefreshService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    // Resolve the scoped price service per tick; never hold a DbContext on this singleton hosted service.
                    using var scope = _scopeFactory.CreateScope();
                    var prices = scope.ServiceProvider.GetRequiredService<IPricesService>();
                    await prices.RefreshAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Scheduled price refresh failed; will retry on next tick.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on shutdown.
        }
    }
}
