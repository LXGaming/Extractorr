using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/fc4f4ab21125cd3817133434acc0c10fba680930/src/NzbDrone.Core/Notifications/Webhook/WebhookMovieFile.cs
public record MovieFile {

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

    [JsonPropertyName("indexerFlags")]
    public string? IndexerFlags { get; init; }

    [JsonPropertyName("size")]
    public long? Size { get; init; }

    [JsonPropertyName("dateAdded")]
    public DateTime? DateAdded { get; init; }

    [JsonPropertyName("mediaInfo")]
    public MovieFileMediaInfo? MediaInfo { get; init; }

    [JsonPropertyName("recycleBinPath")]
    public string? RecycleBinPath { get; init; }
}