using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/fc4f4ab21125cd3817133434acc0c10fba680930/src/NzbDrone.Core/Notifications/Webhook/WebhookGrabbedRelease.cs
public record GrabbedRelease {

    [JsonPropertyName("releaseTitle")]
    public string? ReleaseTitle { get; init; }

    [JsonPropertyName("indexer")]
    public string? Indexer { get; init; }

    [JsonPropertyName("size")]
    public long? Size { get; init; }
}