using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Radarr;
using LXGaming.Extractorr.Server.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LXGaming.Extractorr.Server.Controllers.Webhooks;

[ApiController]
[Route("webhooks/radarr")]
public class RadarrController(RadarrService radarrService) : ControllerBase {

    [HttpPost]
    public async Task<IActionResult> OnPostAsync() {
        if (!Request.VerifyBasicAuthentication(radarrService.Options.Username, radarrService.Options.Password)) {
            return HttpContext.Unauthorized(Constants.AuthenticationSchemes.Basic);
        }

        using var document = await JsonSerializer.DeserializeAsync<JsonDocument>(Request.Body);
        if (document == null) {
            return BadRequest();
        }

        await radarrService.OnWebhookAsync(document);
        return NoContent();
    }
}