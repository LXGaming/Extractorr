using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public record Episode {

    [JsonPropertyName("airDate")]
    public string? AirDate { get; init; }

    [JsonPropertyName("airDateUtc")]
    public DateTime? AirDateUtc { get; init; }

    [JsonPropertyName("episodeNumber")]
    public int EpisodeNumber { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("seasonNumber")]
    public int SeasonNumber { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }
}