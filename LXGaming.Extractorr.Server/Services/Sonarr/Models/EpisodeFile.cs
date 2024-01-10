using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/v4.0.0.748/src/NzbDrone.Core/Notifications/Webhook/WebhookEpisodeFile.cs
public record EpisodeFile {

    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("relativePath")]
    public string? RelativePath { get; init; }

    [JsonPropertyName("path")]
    public string? Path { get; init; }

    [JsonPropertyName("quality")]
    public string? Quality { get; init; }

    [JsonPropertyName("qualityVersion")]
    public int? QualityVersion { get; init; }

    [JsonPropertyName("releaseGroup")]
    public string? ReleaseGroup { get; init; }

    [JsonPropertyName("sceneName")]
    public string? SceneName { get; init; }

    [JsonPropertyName("size")]
    public long? Size { get; init; }

    [JsonPropertyName("dateAdded")]
    public DateTime? DateAdded { get; init; }

    [JsonPropertyName("mediaInfo")]
    public EpisodeFileMediaInfo? MediaInfo { get; init; }
}