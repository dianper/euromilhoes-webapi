namespace euromilhoes.Services;

using euromilhoes.Services.Interfaces;

public class PeriodicHostedService : BackgroundService
{
    private readonly ILogger<PeriodicHostedService> _logger;
    private readonly IEuromilhoesCrawlerService _euromilhoesCrawlerService;
    private readonly TimeSpan _period = TimeSpan.FromHours(1);
    private DateTime _startTime;

    public PeriodicHostedService(
        ILogger<PeriodicHostedService> logger,
        IEuromilhoesCrawlerService euromilhoesCrawlerService)
    {
        _logger = logger;
        _euromilhoesCrawlerService = euromilhoesCrawlerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _startTime = DateTime.Now;

                await _euromilhoesCrawlerService.GetResultsAsync(stoppingToken);
                //await _euromilhoesCrawlerService.GetLastResultAsync(stoppingToken);

                _logger.LogInformation(
                    $"{_startTime.ToString("dd/MM/yyyy HH:mm:ss")} - Executed {nameof(PeriodicHostedService)}. " +
                    $"Next run scheduled for {_startTime.AddHours(_period.TotalHours).ToString("dd/MM/yyyy HH:mm:ss")}");

                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to execute {nameof(PeriodicHostedService)}.", new
                {
                    ex.Message
                });
            }
        }
    }
}
