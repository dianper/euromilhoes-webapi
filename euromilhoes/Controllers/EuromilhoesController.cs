namespace euromilhoes.Controllers;

using euromilhoes.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class EuromilhoesController : ControllerBase
{
    private readonly IEuromilhoesService _euromilhoesService;
    private readonly ILogger<EuromilhoesController> _logger;

    public EuromilhoesController(
        IEuromilhoesService euromilhoesService,
        ILogger<EuromilhoesController> logger)
    {
        _euromilhoesService = euromilhoesService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Last10()
    {
        _logger.LogInformation("Getting last 10 results.");

        return Ok(_euromilhoesService.GetLast10());
    }

    [HttpGet]
    public IActionResult Generate()
    {
        var numbers = _euromilhoesService.GenerateNumbers();

        _logger.LogInformation($"Generated numbers ({numbers}).");

        return Ok(numbers);
    }

    [HttpGet]
    public IActionResult GetByNumbersAndStars(string numbers, string stars)
    {
        if (string.IsNullOrWhiteSpace(numbers))
        {
            throw new ArgumentNullException(nameof(numbers));
        }

        if (string.IsNullOrWhiteSpace(stars))
        {
            throw new ArgumentNullException(nameof(stars));
        }

        _logger.LogInformation($"Checking numbers ({numbers}) & stars ({stars})");

        var num = _euromilhoesService.GetByNumbersAndStars(numbers, stars);

        if (num != null)
        {
            return Ok(num);
        }

        return NotFound();
    }

    [HttpGet]
    public IActionResult Repeated()
    {
        _logger.LogInformation("Getting repeated numbers.");

        return Ok(_euromilhoesService.GetRepeated());
    }
}
