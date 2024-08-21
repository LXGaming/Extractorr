using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

// https://github.com/jesec/flood/blob/7cdf1de10743f6f4bf5f0eb553450f8e2a60e268/shared/types/TorrentContent.ts#L7
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