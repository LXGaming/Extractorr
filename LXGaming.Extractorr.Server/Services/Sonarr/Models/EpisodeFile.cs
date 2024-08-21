using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/Notifications/Webhook/WebhookEpisodeFile.cs
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

    [JsonPropertyName("recycleBinPath")]
    public string? RecycleBinPath { get; init; }
}