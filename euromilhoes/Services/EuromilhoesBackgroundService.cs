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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("StartAsync");

        return this.euromilhoesService.DoCrawlingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("StopAsync");

        return Task.CompletedTask;
    }
}
