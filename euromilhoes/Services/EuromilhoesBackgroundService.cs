namespace euromilhoes.Services;

using euromilhoes.Models;
using euromilhoes.Services.Interfaces;

public class EuromilhoesBackgroundService : IHostedService, IDisposable
{
    private readonly ILogger<EuromilhoesBackgroundService> logger;
    private readonly IEuromilhoesService euromilhoesService;

    public EuromilhoesBackgroundService(
        ILogger<EuromilhoesBackgroundService> logger,
        IEuromilhoesService euromilhoesService)
    {
        this.logger = logger;
        this.euromilhoesService = euromilhoesService;
    }

    public void Dispose()
    {
        this.logger.LogInformation("Dispose");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("StartAsync");

        var range = Enumerable.Range(2004, (DateTime.Now.Year + 1) - 2004);
        var results = new List<EuromilhoesResult>();

        foreach (var year in range)
        {
            results.AddRange(await this.euromilhoesService.GetAsync(year.ToString(), cancellationToken));
        }

        this.euromilhoesService.Results = results
            .OrderByDescending(x => x.Date)
            .ToList();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("StopAsync");

        return Task.CompletedTask;
    }
}
