using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

// https://github.com/jesec/flood/blob/b0968ef9c0cdd7ecabbbab78335706e2643e6bdd/shared/types/TorrentContent.ts#L7
public record TorrentContent {

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("path")]
    public required string Path { get; init; }

    [JsonPropertyName("filename")]
    public required string Filename { get; init; }

    [JsonPropertyName("percentComplete")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public double? PercentComplete { get; init; }

    [JsonPropertyName("priority")]
    public TorrentContentPriority Priority { get; init; }

    [JsonPropertyName("sizeBytes")]
    public long SizeBytes { get; init; }
}