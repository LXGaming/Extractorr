using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class Movie {

    [JsonPropertyName("FilePath")]
    public string? FilePath { get; set; }

    [JsonPropertyName("FolderPath")]
    public string? FolderPath { get; set; }

    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("ImdbId")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("ReleaseDate")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("Title")]
    public string? Title { get; set; }

    [JsonPropertyName("TmdbId")]
    public int TmdbId { get; set; }

    [JsonPropertyName("Year")]
    public int Year { get; set; }
}