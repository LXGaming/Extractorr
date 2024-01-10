using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Sonarr;
using LXGaming.Extractorr.Server.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LXGaming.Extractorr.Server.Controllers.Webhooks;

[ApiController]
[Route("webhooks/sonarr")]
public class SonarrController(SonarrService sonarrService) : ControllerBase {

    [HttpPost]
    public async Task<IActionResult> OnPostAsync() {
        if (!Request.VerifyBasicAuthentication(sonarrService.Options.Username, sonarrService.Options.Password)) {
            return HttpContext.Unauthorized(Constants.AuthenticationSchemes.Basic);
        }

        using var document = await JsonSerializer.DeserializeAsync<JsonDocument>(Request.Body);
        if (document == null) {
            return BadRequest();
        }

        await sonarrService.ExecuteAsync(document);
        return NoContent();
    }
}