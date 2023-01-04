namespace euromilhoes.Services.Interfaces;

using euromilhoes.Models;

public interface IEuromilhoesService
{
    void SetResults(IEnumerable<EuromilhoesResult> results);

    ApiResult<IEnumerable<EuromilhoesResult>> GetLast(int quantity);

    ApiResult<IEnumerable<EuromilhoesResult>> GetRepeated();

    ApiResult<EuromilhoesResult> Get(string key);

    ApiResult<EuromilhoesResult> GenerateKey();
}
