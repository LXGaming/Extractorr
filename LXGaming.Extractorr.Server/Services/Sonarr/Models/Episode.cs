using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public class Episode {

    [JsonPropertyName("AirDate")]
    public string? AirDate { get; set; }

    [JsonPropertyName("AirDateUtc")]
    public DateTime? AirDateUtc { get; set; }

    [JsonPropertyName("EpisodeNumber")]
    public int EpisodeNumber { get; set; }

    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("SeasonNumber")]
    public int SeasonNumber { get; set; }

    [JsonPropertyName("Title")]
    public string? Title { get; set; }
}