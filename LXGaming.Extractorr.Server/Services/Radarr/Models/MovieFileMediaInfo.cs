using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/4c007291833246d3ed78e6f396fc7e60cc9ca70c/src/NzbDrone.Core/Notifications/Webhook/WebhookMovieFileMediaInfo.cs
public record MovieFileMediaInfo {

    [JsonPropertyName("audioChannels")]
    public decimal? AudioChannels { get; init; }

    [JsonPropertyName("audioCodec")]
    public string? AudioCodec { get; init; }

    [JsonPropertyName("audioLanguages")]
    public ImmutableArray<string>? AudioLanguages { get; init; }

    [JsonPropertyName("height")]
    public int? Height { get; init; }

    [JsonPropertyName("width")]
    public int? Width { get; init; }

    [JsonPropertyName("subtitles")]
    public ImmutableArray<string>? Subtitles { get; init; }

    [JsonPropertyName("videoCodec")]
    public string? VideoCodec { get; init; }

    [JsonPropertyName("videoDynamicRange")]
    public string? VideoDynamicRange { get; init; }

    [JsonPropertyName("videoDynamicRangeType")]
    public string? VideoDynamicRangeType { get; init; }
}