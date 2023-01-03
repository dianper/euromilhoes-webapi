namespace euromilhoes.Services.Interfaces;

public interface IEuromilhoesCrawlerService
{
    Task GetLastResultAsync(CancellationToken cancellation = default);

    Task GetResultsAsync(CancellationToken cancellationToken = default);
}
