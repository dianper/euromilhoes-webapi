namespace euromilhoes.Services.Interfaces;

using euromilhoes.Models;

public interface IEuromilhoesService
{
    Task DoCrawlingAsync(CancellationToken cancellationToken = default);

    IEnumerable<EuromilhoesResult> GetLast10();

    IEnumerable<EuromilhoesResult> GetRepeated();

    EuromilhoesResult? GetByNumbers(string numbers);

    string GenerateNumbers();
}
