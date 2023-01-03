namespace euromilhoes.Services;

using euromilhoes.Models;
using euromilhoes.Services.Interfaces;
using HtmlAgilityPack;

public class EuromilhoesCrawlerService : IEuromilhoesCrawlerService
{
    private readonly ILogger<EuromilhoesCrawlerService> _logger;
    private readonly IEuromilhoesService _euromilhoesService;
    private readonly HttpClient _client;
    private static readonly HtmlDocument _htmlDocument = new();
    private const int Start = 2004;

    public EuromilhoesCrawlerService(
        ILogger<EuromilhoesCrawlerService> logger,
        IEuromilhoesService euromilhoesService,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _euromilhoesService = euromilhoesService;
        _client = httpClientFactory.CreateClient("euromilhoes");
    }

    public async Task GetResultsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var years = Enumerable.Range(Start, (DateTime.Now.Year + 1) - Start);

            var tasks = years.Select(y => _client.GetAsync("resultados-euromilhoes.asp?y=" + y, cancellationToken));
            var responses = await Task.WhenAll(tasks);

            var tasksOfString = responses.Select(response => response.Content.ReadAsStringAsync(cancellationToken));
            var htmls = await Task.WhenAll(tasksOfString);

            _euromilhoesService.SetResults(htmls.SelectMany(x => ParseResultsHtml(x)));
        }
        catch (Exception ex)
        {
            _logger.LogError("Error getting results from web site.", new
            {
                ex.Message
            });
        }
    }

    public async Task<EuromilhoesResult?> GetLastResultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var httpResponseMessage = await _client.GetAsync("resultado-euromilhoes.asp", cancellationToken);
            var html = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

            return ParseLastResult(html); ;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error getting last result from web site.", new
            {
                ex.Message
            });

            return default;
        }
    }

    private static IEnumerable<EuromilhoesResult> ParseResultsHtml(string html)
    {
        _htmlDocument.LoadHtml(html);

        var nodes = _htmlDocument.DocumentNode.SelectNodes("//table[@class='tbl no-responsive ee hover no-back']//tr");
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

    private static EuromilhoesResult? ParseLastResult(string html)
    {
        _htmlDocument.LoadHtml(html);

        var dateString = string.Empty;
        var date = (DateTime)default;
        var dateHtmlNode = _htmlDocument.DocumentNode.SelectSingleNode("//div[@class='date arrowBottom']//span[@class='show-maxphablet']");
        if (dateHtmlNode != null)
        {
            dateString = ReplaceMonth(dateHtmlNode.InnerText.Replace("&nbsp;", " ").AsSpan(3).Trim().ToString());
            date = DateTime.Parse(dateString);
        }

        var nodes = _htmlDocument.DocumentNode.SelectNodes("//div[@class='combi balls']//div//span[@class='int-num']");
        if (nodes != null)
        {
            var numbers = string.Join("-", nodes.Take(5).Select(s => s.InnerText.PadLeft(2, '0')));
            var stars = string.Join("-", nodes.TakeLast(2).Select(s => s.InnerText.PadLeft(2, '0')));

            return new EuromilhoesResult(dateString, date, "", numbers, stars);
        }

        return default;
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
