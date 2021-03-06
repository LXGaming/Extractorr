using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class MovieFile {

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("indexerFlags")]
    public string? IndexerFlags { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("quality")]
    public string? Quality { get; set; }

    [JsonPropertyName("qualityVersion")]
    public int QualityVersion { get; set; }

    [JsonPropertyName("relativePath")]
    public string? RelativePath { get; set; }

    [JsonPropertyName("releaseGroup")]
    public string? ReleaseGroup { get; set; }

    [JsonPropertyName("sceneName")]
    public string? SceneName { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }
}