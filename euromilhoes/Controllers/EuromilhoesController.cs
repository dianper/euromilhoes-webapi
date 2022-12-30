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
        return Ok(this.euromilhoesService.Results.Take(10).Select(x => new
        {
            date = x.DateString,
            value = x.Value,
            numbers = x.Numbers,
            starts = x.Stars
        }));
    }

    [HttpGet]
    public IActionResult Generate()
    {
        var numbers = "";

        while (numbers.Length == 0)
        {
            var nums = this.euromilhoesService.GenerateNumbers();

            if (this.euromilhoesService.GetByNumbers(nums) == null)
            {
                numbers = nums;
            }
        }

        return Ok(numbers);
    }

    [HttpGet]
    public IActionResult Exists(string numbers)
    {
        if (string.IsNullOrWhiteSpace(numbers))
        {
            throw new ArgumentNullException(nameof(numbers));
        }

        var result = this.euromilhoesService.GetByNumbers(numbers);

        return Ok(new
        {
            exists = result != null,
            result
        });
    }
}
