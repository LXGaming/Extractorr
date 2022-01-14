using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class RemoteMovie {

    [JsonPropertyName("imdbId")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("tmdbId")]
    public int TmdbId { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }
}