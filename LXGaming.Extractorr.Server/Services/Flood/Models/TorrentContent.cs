using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

// https://github.com/jesec/flood/blob/77f4bc7267331f2c731c47dd62b570d4f0bf0c1d/shared/types/TorrentContent.ts#L7
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