using Microsoft.AspNetCore.Mvc;

namespace LXGaming.Extractorr.Server.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase {

    [HttpGet]
    public IActionResult OnGet() {
        return Ok(new {
            Application = "Extractorr API"
        });
    }
}