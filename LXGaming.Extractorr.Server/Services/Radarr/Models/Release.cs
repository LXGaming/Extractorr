using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public record Release {

    [JsonPropertyName("indexer")]
    public string? Indexer { get; init; }

    [JsonPropertyName("quality")]
    public string? Quality { get; init; }

    [JsonPropertyName("qualityVersion")]
    public int QualityVersion { get; init; }

    [JsonPropertyName("releaseGroup")]
    public string? ReleaseGroup { get; init; }

    [JsonPropertyName("releaseTitle")]
    public string? ReleaseTitle { get; init; }

    [JsonPropertyName("size")]
    public long Size { get; init; }
}