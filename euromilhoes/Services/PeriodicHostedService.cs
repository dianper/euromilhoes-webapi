namespace euromilhoes.Services;

using euromilhoes.Services.Interfaces;

public class PeriodicHostedService : BackgroundService
{
    private readonly ILogger<PeriodicHostedService> _logger;
    private readonly IEuromilhoesCrawlerService _euromilhoesCrawlerService;
    private readonly IEuromilhoesService _euromilhoesService;
    private readonly TimeSpan _period = TimeSpan.FromHours(1);
    private DateTime _startTime;

    public PeriodicHostedService(
        ILogger<PeriodicHostedService> logger,
        IEuromilhoesCrawlerService euromilhoesCrawlerService,
        IEuromilhoesService euromilhoesService)
    {
        _logger = logger;
        _euromilhoesCrawlerService = euromilhoesCrawlerService;
        _euromilhoesService = euromilhoesService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _startTime = DateTime.Now;

                _logger.LogInformation($"Execution started: {_startTime:dd/MM/yyyy HH:mm:ss}");

                var lastResult = await _euromilhoesCrawlerService.GetLastResultAsync(stoppingToken);
                if (lastResult != null)
                {
                    if (!_euromilhoesService.Exists(lastResult.Numbers, lastResult.Stars))
                    {
                        _logger.LogInformation("Getting results from the website.");

                        await _euromilhoesCrawlerService.GetResultsAsync(stoppingToken);
                    }
                    else
                    {
                        _logger.LogInformation("There are no results to update.");
                    }
                }

                _logger.LogInformation($"Next run scheduled for {_startTime.AddHours(_period.TotalHours):dd/MM/yyyy HH:mm:ss}");

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
