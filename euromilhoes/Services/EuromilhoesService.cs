namespace euromilhoes.Services;

using euromilhoes.Models;
using euromilhoes.Services.Interfaces;
using System;

public class EuromilhoesService : IEuromilhoesService
{
    private IReadOnlyList<EuromilhoesResult> _results;

    public void SetResults(IEnumerable<EuromilhoesResult> results) =>
        _results = results.ToList();

    public EuromilhoesService() =>
        _results = new List<EuromilhoesResult>();

    public ApiResult<IEnumerable<EuromilhoesResult>> GetLast(int quantity)
    {
        if (quantity > 100)
        {
            return new ApiResult<IEnumerable<EuromilhoesResult>>()
            {
                Success = false,
                Message = "Quantity cannot be greater than 100!"
            };
        }

        var data = _results.OrderByDescending(d => d.Date).Take(quantity);

        return new ApiResult<IEnumerable<EuromilhoesResult>>(data);
    }

    public ApiResult<IEnumerable<EuromilhoesResult>> GetRepeated()
    {
        var data = _results
            .GroupBy(s => s.Numbers)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x);

        return new ApiResult<IEnumerable<EuromilhoesResult>>(data);
    }

    public ApiResult<EuromilhoesResult> GetByKey(string key)
    {
        var values = key.Split(new char[] { ',', '-' });
        var isValid = values.All(x => int.TryParse(x, out var num) && num > 0 && num <= 50 ? true : false);
        if (!isValid)
        {
            return new ApiResult<EuromilhoesResult>()
            {
                Success = false,
                Message = "The argument must be an array of numbers between 1 and 50!"
            };
        }

        if (values.Length != 7)
        {
            return new ApiResult<EuromilhoesResult>()
            {
                Success = false,
                Message = "The key must be 7 digits long!"
            };
        }

        if (!values.TakeLast(2).All(x => int.Parse(x) > 0 && int.Parse(x) <= 12))
        {
            return new ApiResult<EuromilhoesResult>()
            {
                Success = false,
                Message = "The stars must be between 1 and 12!"
            };
        }

        var numbers = string.Join("-", values.Take(5).OrderBy(x => x).Select(x => x.ToString().PadLeft(2, '0')));
        var stars = string.Join("-", values.TakeLast(2).OrderBy(x => x).Select(x => x.ToString().PadLeft(2, '0')));
        var data = _results.FirstOrDefault(x => x.Numbers.Equals(numbers) && x.Stars.Equals(stars));

        return new ApiResult<EuromilhoesResult>(data);
    }

    public ApiResult<EuromilhoesResult> GenerateKey()
    {
        var random = new Random();
        var numbers = GenerateValue(random, 5, 50);
        var stars = GenerateValue(random, 2, 12);

        if (this.GetByKey($"{numbers}-{stars}").Data == null)
        {
            return new ApiResult<EuromilhoesResult>(
                new EuromilhoesResult(
                    DateTime.UtcNow.ToString(),
                    DateTime.UtcNow,
                    string.Empty,
                    numbers,
                    stars));
        }

        return GenerateKey();
    }

    private static string GenerateValue(Random random, int capacity, int maxValue)
    {
        var hash = new HashSet<int>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            int num;
            do
            {
                num = random.Next(1, maxValue);
            } while (!hash.Add(num));
        }

        return string.Join("-", hash.OrderBy(x => x).Select(x => x.ToString().PadLeft(2, '0')));
    }
}
