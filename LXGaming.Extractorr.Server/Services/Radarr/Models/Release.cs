using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class Release {

    [JsonPropertyName("Indexer")]
    public string? Indexer { get; set; }

    [JsonPropertyName("Quality")]
    public string? Quality { get; set; }

    [JsonPropertyName("QualityVersion")]
    public int QualityVersion { get; set; }

    [JsonPropertyName("ReleaseGroup")]
    public string? ReleaseGroup { get; set; }

    [JsonPropertyName("ReleaseTitle")]
    public string? ReleaseTitle { get; set; }

    [JsonPropertyName("Size")]
    public long Size { get; set; }
}