using euromilhoes.Models;

namespace euromilhoes.Services.Interfaces
{
    public interface IEuromilhoesService
    {
        Task<IEnumerable<EuromilhoesResult>> GetAsync(string year, CancellationToken cancellationToken = default);

        IReadOnlyList<EuromilhoesResult> Results { get; set; }

        string GenerateNumbers();

        EuromilhoesResult? GetByNumbers(string numbers);
    }
}
