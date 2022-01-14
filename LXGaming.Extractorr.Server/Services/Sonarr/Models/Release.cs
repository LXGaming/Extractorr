using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public class Release {

    [JsonPropertyName("indexer")]
    public string? Indexer { get; set; }

    [JsonPropertyName("quality")]
    public string? Quality { get; set; }

    [JsonPropertyName("qualityVersion")]
    public int QualityVersion { get; set; }

    [JsonPropertyName("releaseGroup")]
    public string? ReleaseGroup { get; set; }

    [JsonPropertyName("releaseTitle")]
    public string? ReleaseTitle { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }
}