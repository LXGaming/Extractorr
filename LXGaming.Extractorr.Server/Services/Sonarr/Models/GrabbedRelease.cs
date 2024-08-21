using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/Notifications/Webhook/WebhookGrabbedRelease.cs
public record GrabbedRelease {

    [JsonPropertyName("releaseTitle")]
    public string? ReleaseTitle { get; init; }

    [JsonPropertyName("indexer")]
    public string? Indexer { get; init; }

    [JsonPropertyName("size")]
    public long? Size { get; init; }

    [JsonPropertyName("releaseType")]
    public ReleaseType? ReleaseType { get; init; }
}