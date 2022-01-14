using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class RemoteMovie {

    [JsonPropertyName("ImdbId")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("Title")]
    public string? Title { get; set; }

    [JsonPropertyName("TmdbId")]
    public int TmdbId { get; set; }

    [JsonPropertyName("Year")]
    public int Year { get; set; }
}