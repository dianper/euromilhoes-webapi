namespace euromilhoes.Services;

using euromilhoes.Models;
using euromilhoes.Services.Interfaces;
using HtmlAgilityPack;

public class EuromilhoesService : IEuromilhoesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EuromilhoesService> _logger;

    public EuromilhoesService(
        IHttpClientFactory httpClientFactory,
        ILogger<EuromilhoesService> logger)
    {
        this._httpClientFactory = httpClientFactory;
        this._logger = logger;

        this.Results = new List<EuromilhoesResult>();
    }

    public IReadOnlyList<EuromilhoesResult> Results { get; set; }

    public async Task<IEnumerable<EuromilhoesResult>> GetAsync(string year, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = this._httpClientFactory.CreateClient("euromilhoes");
            var httpResponseMessage = await client.GetAsync("resultados-euromilhoes.asp?y=" + year, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            var html = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrEmpty(html))
            {
                return Enumerable.Empty<EuromilhoesResult>();
            }

            return ParseHtml(html);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex?.Message);

            return Enumerable.Empty<EuromilhoesResult>();
        }
    }

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

        return string.Join("-", result.OrderBy(x => x).Select(x => x.ToString().PadLeft(2, '0')));
    }

    public EuromilhoesResult? GetByNumbers(string numbers) =>
        this.Results
            .FirstOrDefault(x => x.Numbers.Equals(numbers));

    public IEnumerable<EuromilhoesResult> GetRepeated() =>
        this.Results
            .GroupBy(s => s.Numbers)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x);

    private IEnumerable<EuromilhoesResult> ParseHtml(string html)
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
