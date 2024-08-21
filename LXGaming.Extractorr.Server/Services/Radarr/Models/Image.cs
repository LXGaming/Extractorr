using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/fc4f4ab21125cd3817133434acc0c10fba680930/src/NzbDrone.Core/Notifications/Webhook/WebhookImage.cs
public record Image {

    [JsonPropertyName("coverType")]
    public CoverType? CoverType { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("remoteUrl")]
    public string? RemoteUrl { get; init; }
}