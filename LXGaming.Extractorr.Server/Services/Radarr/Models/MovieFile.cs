using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class MovieFile {

    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("IndexerFlags")]
    public string? IndexerFlags { get; set; }

    [JsonPropertyName("Path")]
    public string? Path { get; set; }

    [JsonPropertyName("Quality")]
    public string? Quality { get; set; }

    [JsonPropertyName("QualityVersion")]
    public int QualityVersion { get; set; }

    [JsonPropertyName("RelativePath")]
    public string? RelativePath { get; set; }

    [JsonPropertyName("ReleaseGroup")]
    public string? ReleaseGroup { get; set; }

    [JsonPropertyName("SceneName")]
    public string? SceneName { get; set; }

    [JsonPropertyName("Size")]
    public long Size { get; set; }
}