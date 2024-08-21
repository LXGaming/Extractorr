using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/fc4f4ab21125cd3817133434acc0c10fba680930/src/NzbDrone.Core/Notifications/Webhook/WebhookRelease.cs
public record Release {

    [JsonPropertyName("quality")]
    public string? Quality { get; init; }

    [JsonPropertyName("qualityVersion")]
    public int? QualityVersion { get; init; }

    [JsonPropertyName("releaseGroup")]
    public string? ReleaseGroup { get; init; }

    [JsonPropertyName("releaseTitle")]
    public string? ReleaseTitle { get; init; }

    [JsonPropertyName("indexer")]
    public string? Indexer { get; init; }

    [JsonPropertyName("size")]
    public long? Size { get; init; }

    [JsonPropertyName("customFormatScore")]
    public int? CustomFormatScore { get; init; }

    [JsonPropertyName("customFormats")]
    public List<string>? CustomFormats { get; init; }

    [JsonPropertyName("indexerFlags")]
    public List<string>? IndexerFlags { get; init; }
}