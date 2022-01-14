using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Sonarr;
using LXGaming.Extractorr.Server.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LXGaming.Extractorr.Server.Controllers.Webhooks;

[ApiController]
[Route("webhooks/sonarr")]
public class SonarrController : ControllerBase {

    private readonly SonarrService _sonarrService;

    public SonarrController(SonarrService sonarrService) {
        _sonarrService = sonarrService;
    }

    [HttpPost]
    public async Task<IActionResult> OnPostAsync() {
        if (!Request.VerifyBasicAuthentication(_sonarrService.Options.Username, _sonarrService.Options.Password)) {
            return HttpContext.Unauthorized(Constants.AuthenticationSchemes.Basic);
        }

        using var document = await JsonDocument.ParseAsync(Request.Body);
        await _sonarrService.ExecuteAsync(document);
        return NoContent();
    }
}