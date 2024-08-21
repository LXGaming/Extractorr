using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/Notifications/Webhook/WebhookEpisodeFileMediaInfo.cs
public record EpisodeFileMediaInfo {

    [JsonPropertyName("audioChannels")]
    public decimal? AudioChannels { get; init; }

    [JsonPropertyName("audioCodec")]
    public string? AudioCodec { get; init; }

    [JsonPropertyName("audioLanguages")]
    public List<string>? AudioLanguages { get; init; }

    [JsonPropertyName("height")]
    public int? Height { get; init; }

    [JsonPropertyName("width")]
    public int? Width { get; init; }

    [JsonPropertyName("subtitles")]
    public List<string>? Subtitles { get; init; }

    [JsonPropertyName("videoCodec")]
    public string? VideoCodec { get; init; }

    [JsonPropertyName("videoDynamicRange")]
    public string? VideoDynamicRange { get; init; }

    [JsonPropertyName("videoDynamicRangeType")]
    public string? VideoDynamicRangeType { get; init; }
}