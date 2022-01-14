using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class Movie {

    [JsonPropertyName("filePath")]
    public string? FilePath { get; set; }

    [JsonPropertyName("folderPath")]
    public string? FolderPath { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("imdbId")]
    public string? ImdbId { get; set; }

    [JsonPropertyName("releaseDate")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("tmdbId")]
    public int TmdbId { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }
}