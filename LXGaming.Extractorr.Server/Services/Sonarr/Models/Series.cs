using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/Notifications/Webhook/WebhookSeries.cs
public record Series {

    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("titleSlug")]
    public string? TitleSlug { get; init; }

    [JsonPropertyName("path")]
    public string? Path { get; init; }

    [JsonPropertyName("tvdbId")]
    public int? TvdbId { get; init; }

    [JsonPropertyName("tvMazeId")]
    public int? TvMazeId { get; init; }

    [JsonPropertyName("tmdbId")]
    public int? TmdbId { get; init; }

    [JsonPropertyName("imdbId")]
    public string? ImdbId { get; init; }

    [JsonPropertyName("type")]
    public SeriesType? Type { get; init; }

    [JsonPropertyName("year")]
    public int? Year { get; init; }

    [JsonPropertyName("genres")]
    public List<string>? Genres { get; init; }

    [JsonPropertyName("images")]
    public List<Image>? Images { get; init; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; init; }
}