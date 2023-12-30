using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public record Series {

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("imdbId")]
    public string? ImdbId { get; init; }

    [JsonPropertyName("path")]
    public string? Path { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("tvdbId")]
    public int TvdbId { get; init; }

    [JsonPropertyName("tvMazeId")]
    public int TvMazeId { get; init; }

    [JsonPropertyName("type")]
    public SeriesType Type { get; init; }
}