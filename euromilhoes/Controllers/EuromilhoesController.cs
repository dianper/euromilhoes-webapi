namespace euromilhoes.Controllers;

using euromilhoes.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class EuromilhoesController : ControllerBase
{
    private readonly IEuromilhoesService euromilhoesService;

    public EuromilhoesController(IEuromilhoesService euromilhoesService)
    {
        this.euromilhoesService = euromilhoesService;
    }

    [HttpGet]
    public IActionResult Last10()
    {
        return Ok(this.euromilhoesService.GetLast10());
    }

    [HttpGet]
    public IActionResult Generate()
    {
        return Ok(this.euromilhoesService.GenerateNumbers());
    }

    [HttpGet]
    public IActionResult Exists(string numbers)
    {
        if (string.IsNullOrWhiteSpace(numbers))
        {
            throw new ArgumentNullException(nameof(numbers));
        }

        return Ok(this.euromilhoesService.GetByNumbers(numbers));
    }

    [HttpGet]
    public IActionResult Repeated()
    {
        return Ok(this.euromilhoesService.GetRepeated());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(CancellationToken cancellation = default)
    {
        await this.euromilhoesService.DoCrawlingAsync(cancellation);

        return NoContent();
    }
}
