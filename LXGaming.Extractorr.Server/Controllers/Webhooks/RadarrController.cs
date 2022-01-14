using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Radarr;
using LXGaming.Extractorr.Server.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LXGaming.Extractorr.Server.Controllers.Webhooks;

[ApiController]
[Route("webhooks/radarr")]
public class RadarrController : ControllerBase {

    private readonly RadarrService _radarrService;

    public RadarrController(RadarrService radarrService) {
        _radarrService = radarrService;
    }

    [HttpPost]
    public async Task<IActionResult> OnPostAsync() {
        if (!Request.VerifyBasicAuthentication(_radarrService.Options.Username, _radarrService.Options.Password)) {
            return HttpContext.Unauthorized(Constants.AuthenticationSchemes.Basic);
        }

        using var document = await JsonDocument.ParseAsync(Request.Body);
        await _radarrService.ExecuteAsync(document);
        return NoContent();
    }
}