using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public class Series {

    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("ImdbId")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("Path")]
    public string? Path { get; set; }

    [JsonPropertyName("Title")]
    public string? Title { get; set; }

    [JsonPropertyName("TvdbId")]
    public int TvdbId { get; set; }

    [JsonPropertyName("TvMazeId")]
    public int TvMazeId { get; set; }

    [JsonPropertyName("Type")]
    public SeriesType Type { get; set; }
}