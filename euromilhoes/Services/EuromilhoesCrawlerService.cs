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

    public async Task GetLastResultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var httpResponseMessage = await _client.GetAsync("resultado-euromilhoes.asp", cancellationToken);
            var html = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

            ParseLastResult(html); ;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error getting last result from web site.", new
            {
                ex.Message
            });
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

    private static EuromilhoesResult ParseLastResult(string html)
    {
        _htmlDocument.LoadHtml(html);

        var nodes = _htmlDocument.DocumentNode.SelectNodes("//div[@class='combi balls']//div//span[@class='int-num']");
        if (nodes != null)
        {
            var num1 = nodes[0].InnerText;
            var num2 = nodes[1].InnerText;
            var num3 = nodes[2].InnerText;
            var num4 = nodes[3].InnerText;
            var num5 = nodes[4].InnerText;
            var start1 = nodes[5].InnerText;
            var start2 = nodes[6].InnerText;
        }

        return new EuromilhoesResult(default, default, default, default, default);
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
