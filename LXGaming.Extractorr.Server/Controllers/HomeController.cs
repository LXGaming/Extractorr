using LXGaming.Extractorr.Server.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LXGaming.Extractorr.Server.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase {

    [HttpGet]
    public IActionResult OnGet() {
        return Ok(new {
            Id = Constants.Application.Id,
            Name = Constants.Application.Name,
            Version = Constants.Application.Version,
            Authors = Constants.Application.Authors,
            Source = Constants.Application.Source,
            Website = Constants.Application.Website,
        });
    }
}