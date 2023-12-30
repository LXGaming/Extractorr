using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public record TorrentContent {

    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("percentComplete")]
    public decimal? PercentComplete { get; set; }

    [JsonPropertyName("priority")]
    public TorrentContentPriority Priority { get; set; }

    [JsonPropertyName("sizeBytes")]
    public long SizeBytes { get; set; }
}