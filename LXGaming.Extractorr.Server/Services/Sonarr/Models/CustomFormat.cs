using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Notifications/Webhook/WebhookCustomFormat.cs
public record CustomFormat {

    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}