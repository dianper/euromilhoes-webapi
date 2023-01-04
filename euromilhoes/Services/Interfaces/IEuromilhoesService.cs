namespace euromilhoes.Services.Interfaces;

using euromilhoes.Models;

public interface IEuromilhoesService
{
    void SetResults(IEnumerable<EuromilhoesResult> results);

    ApiResult<IEnumerable<EuromilhoesResult>> GetLast(int quantity);

    ApiResult<IEnumerable<EuromilhoesResult>> GetRepeated();

    ApiResult<EuromilhoesResult> GetByKey(string key);

    ApiResult<bool> Exists(string numbers, string stars);

    ApiResult<EuromilhoesResult> GenerateKey();
}
