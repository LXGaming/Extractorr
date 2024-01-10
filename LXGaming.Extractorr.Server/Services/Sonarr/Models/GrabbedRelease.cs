using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/v4.0.0.748/src/NzbDrone.Core/Notifications/Webhook/WebhookGrabbedRelease.cs
public record GrabbedRelease {

    [JsonPropertyName("releaseTitle")]
    public string? ReleaseTitle { get; init; }

    [JsonPropertyName("indexer")]
    public string? Indexer { get; init; }

    [JsonPropertyName("size")]
    public long? Size { get; init; }
}