using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public record TorrentContent {

    [JsonPropertyName("filename")]
    public string? Filename { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("path")]
    public string? Path { get; init; }

    [JsonPropertyName("percentComplete")]
    public decimal? PercentComplete { get; init; }

    [JsonPropertyName("priority")]
    public TorrentContentPriority Priority { get; init; }

    [JsonPropertyName("sizeBytes")]
    public long SizeBytes { get; init; }
}