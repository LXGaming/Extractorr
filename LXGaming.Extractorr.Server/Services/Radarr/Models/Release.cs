using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/4c007291833246d3ed78e6f396fc7e60cc9ca70c/src/NzbDrone.Core/Notifications/Webhook/WebhookRelease.cs
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
    public ImmutableArray<string>? CustomFormats { get; init; }

    [JsonPropertyName("languages")]
    public ImmutableArray<Language>? Languages { get; init; }

    [JsonPropertyName("indexerFlags")]
    public ImmutableArray<string>? IndexerFlags { get; init; }
}