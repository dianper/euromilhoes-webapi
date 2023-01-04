namespace euromilhoes.Controllers;

using euromilhoes.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

    [HttpGet("last/{quantity}")]
    public IActionResult Last(int quantity)
    {
        _logger.LogInformation($"Getting last {quantity} results.");

        return Ok(_euromilhoesService.GetLast(quantity));
    }

    [HttpGet("generate")]
    public IActionResult Generate()
    {
        var result = _euromilhoesService.GenerateKey();

        _logger.LogInformation($"Generated key {result.Data?.Numbers} :: {result.Data?.Stars}.");

        return Ok(result);
    }

    [HttpGet("key/{key}")]
    public IActionResult GetByKey(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        _logger.LogInformation($"Checking key {key}");

        return Ok(_euromilhoesService.GetByKey(key));
    }

    [HttpGet("repeated")]
    public IActionResult Repeated()
    {
        _logger.LogInformation("Getting repeated numbers.");

        return Ok(_euromilhoesService.GetRepeated());
    }
}
