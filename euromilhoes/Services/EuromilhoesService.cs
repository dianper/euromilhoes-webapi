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

    public IEnumerable<EuromilhoesResult> GetLast10() =>
        _results
            .OrderByDescending(d => d.Date).Take(10);

    public IEnumerable<EuromilhoesResult> GetRepeated() =>
        _results
            .GroupBy(s => s.Numbers)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x);

    public EuromilhoesResult? GetByKey(string numbers, string stars) =>
        _results
            .FirstOrDefault(x => x.Numbers.Equals(numbers) && x.Stars.Equals(stars));

    public bool Exists(string numbers, string stars) =>
        _results
            .FirstOrDefault(x => x.Numbers.Equals(numbers) && x.Stars.Equals(stars)) != null;

    public (string num, string star) GenerateKey()
    {
        var random = new Random();
        var numbers = GenerateValue(random, 5, 50);
        var stars = GenerateValue(random, 2, 12);

        if (!this.Exists(numbers, stars))
        {
            return (numbers, stars);
        }

        return GenerateKey();
    }

    private string GenerateValue(Random random, int capacity, int maxValue)
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
