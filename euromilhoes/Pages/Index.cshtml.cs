namespace euromilhoes.Pages;

using euromilhoes.Models;
using euromilhoes.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly IEuromilhoesService _euromilhoesService;

    public IEnumerable<EuromilhoesResult> Results;

    public IndexModel(IEuromilhoesService euromilhoesService)
    {
        _euromilhoesService = euromilhoesService;
    }

    public void OnGet()
    {
        Results = _euromilhoesService.GetLast10();
    }
}
