namespace euromilhoes.Services.Interfaces;

using euromilhoes.Models;

public interface IEuromilhoesService
{
    void SetResults(IEnumerable<EuromilhoesResult> results);

    IEnumerable<EuromilhoesResult> GetLast10();

    IEnumerable<EuromilhoesResult> GetRepeated();

    EuromilhoesResult? GetByNumbers(string numbers);

    string GenerateNumbers();
}
