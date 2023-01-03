namespace euromilhoes.Services.Interfaces;

using euromilhoes.Models;

public interface IEuromilhoesCrawlerService
{
    Task<EuromilhoesResult?> GetLastResultAsync(CancellationToken cancellation = default);

    Task GetResultsAsync(CancellationToken cancellationToken = default);
}
