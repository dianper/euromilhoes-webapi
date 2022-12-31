namespace euromilhoes.Services;

using euromilhoes.Models;
using euromilhoes.Services.Interfaces;
using HtmlAgilityPack;

public class EuromilhoesService : IEuromilhoesService
{
    private readonly HttpClient client;
    private readonly ILogger<EuromilhoesService> logger;
    private IReadOnlyList<EuromilhoesResult> results;
    private const int Start = 2004;

    public EuromilhoesService(
        IHttpClientFactory httpClientFactory,
        ILogger<EuromilhoesService> logger)
    {
        this.client = httpClientFactory.CreateClient("euromilhoes");
        this.logger = logger;
        this.results = new List<EuromilhoesResult>();
    }

    public async Task DoCrawlingAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var years = Enumerable.Range(Start, (DateTime.Now.Year + 1) - Start);

            var tasks = years.Select(y => client.GetAsync("resultados-euromilhoes.asp?y=" + y, cancellationToken));
            var responses = await Task.WhenAll(tasks);

            var tasksOfString = responses.Select(response => response.Content.ReadAsStringAsync(cancellationToken));
            var htmls = await Task.WhenAll(tasksOfString);

            this.results = htmls
                .SelectMany(x => ParseHtml(x))
                .ToList();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
        }
    }

    public IEnumerable<EuromilhoesResult> GetLast10() => this.results.OrderByDescending(d => d.Date).Take(10);

    public IEnumerable<EuromilhoesResult> GetRepeated() =>
        this.results
            .GroupBy(s => s.Numbers)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x);

    public EuromilhoesResult? GetByNumbers(string numbers) =>
        this.results
            .FirstOrDefault(x => x.Numbers.Equals(numbers));

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
        
        if (this.GetByNumbers(numbers) == null)
        {
            return numbers;
        }

        return GenerateNumbers();
    }

    private static IEnumerable<EuromilhoesResult> ParseHtml(string html)
    {
        var htmlSnippet = new HtmlDocument();
        htmlSnippet.LoadHtml(html);

        var nodes = htmlSnippet.DocumentNode.SelectNodes("//table[@class='tbl no-responsive ee hover no-back']//tr");
        if (nodes != null)
        {
            foreach (HtmlNode item in nodes)
            {
                var columns = item.ChildNodes.Where(x => x.Name == "td").ToArray();
                var dateString = columns[0].InnerText.Replace("&nbsp;", " ").AsSpan(3).Trim();
                var date = DateTime.Parse(ReplaceMonth(dateString.ToString()));
                var value = columns[2].ChildNodes.FirstOrDefault()?.InnerText.Replace("&nbsp;", " ").Replace("&euro;", "€") ?? string.Empty;
                var numbers = columns[10].InnerText;
                var starts = columns[11].InnerText;
                yield return new EuromilhoesResult(dateString.ToString(), date, value, numbers, starts);
            }
        }
    }

    private static string ReplaceMonth(string text)
    {
        return text.ToLower()
            .Replace("fev", "feb")
            .Replace("abr", "apr")
            .Replace("mai", "may")
            .Replace("ago", "aug")
            .Replace("set", "sep")
            .Replace("out", "oct")
            .Replace("dez", "dec");
    }
}
