namespace euromilhoes.Services;

using euromilhoes.Models;
using euromilhoes.Services.Interfaces;

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

    public EuromilhoesResult? GetByNumbersAndStars(string numbers, string stars) =>
        _results
            .FirstOrDefault(x => x.Numbers.Equals(numbers) && x.Stars.Equals(stars));

    public string GenerateNumbers()
    {
        var random = new Random();
        var result = new HashSet<int>(5);

        for (int i = 0; i < 5; i++)
        {
            var num = 0;
            do
            {
                num = random.Next(1, 50);

                // TODO: check if the number is between the ones that leave the most

            } while (!result.Add(num));
        }

        var numbers = string.Join("-", result.OrderBy(x => x).Select(x => x.ToString().PadLeft(2, '0')));

        if (this.GetByNumbersAndStars(numbers, string.Empty) == null)
        {
            return numbers;
        }

        return GenerateNumbers();
    }
}
