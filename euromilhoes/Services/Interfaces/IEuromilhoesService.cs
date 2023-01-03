namespace euromilhoes.Services.Interfaces;

using euromilhoes.Models;

public interface IEuromilhoesService
{
    void SetResults(IEnumerable<EuromilhoesResult> results);

    IEnumerable<EuromilhoesResult> GetLast10();

    IEnumerable<EuromilhoesResult> GetRepeated();

    EuromilhoesResult? GetByNumbersAndStars(string numbers, string stars);

    bool Exists(string numbers, string stars);

    string GenerateNumbers();
}
