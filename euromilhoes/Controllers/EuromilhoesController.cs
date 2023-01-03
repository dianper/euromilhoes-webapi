namespace euromilhoes.Controllers;

using euromilhoes.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
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

    [HttpGet("/last10")]
    public IActionResult Last10()
    {
        _logger.LogInformation("Getting last 10 results.");

        return Ok(_euromilhoesService.GetLast10());
    }

    [HttpGet("/generate")]
    public IActionResult GenerateKey()
    {
        var numbers = _euromilhoesService.GenerateKey();

        _logger.LogInformation($"Generated numbers ({numbers}).");

        return Ok(numbers);
    }

    [HttpGet("/key/{key}")]
    public IActionResult GetByKey(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var values = key.Split(new char[] { ',', '-' });
        var isValid = values.All(x => int.TryParse(x, out var num) && num > 0 && num <= 50 ? true : false);
        if (!isValid)
        {
            return BadRequest(new { isValid, message = "The argument must be an array of numbers between 1 and 50!" });
        }

        if (values.Length != 7)
        {
            return BadRequest(new { isValid = false, message = "The key must be 7 digits long!" });
        }

        var numbers = string.Join("-", values.Take(5).OrderBy(x => x).Select(x => x.ToString().PadLeft(2, '0')));

        if (!values.TakeLast(2).All(x => int.Parse(x) > 0 && int.Parse(x) <= 12))
        {
            return BadRequest(new { isValid, message = "The stars must be between 1 and 12!" });
        }

        var stars = string.Join("-", values.TakeLast(2).OrderBy(x => x).Select(x => x.ToString().PadLeft(2, '0')));

        _logger.LogInformation($"Checking key {key}");

        var num = _euromilhoesService.GetByKey(numbers, stars);
        if (num != null)
        {
            return Ok(new { isValid = true, result = num });
        }

        return NotFound(new { isValid = true, message = "Key not found!" });
    }

    [HttpGet("/repeated")]
    public IActionResult Repeated()
    {
        _logger.LogInformation("Getting repeated numbers.");

        return Ok(_euromilhoesService.GetRepeated());
    }
}
