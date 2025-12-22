using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/4c007291833246d3ed78e6f396fc7e60cc9ca70c/src/NzbDrone.Core/Notifications/Webhook/WebhookMovie.cs
public record Movie {

    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("year")]
    public int? Year { get; init; }

    [JsonPropertyName("filePath")]
    public string? FilePath { get; init; }

    [JsonPropertyName("releaseDate")]
    public string? ReleaseDate { get; init; }

    [JsonPropertyName("folderPath")]
    public string? FolderPath { get; init; }

    [JsonPropertyName("tmdbId")]
    public int? TmdbId { get; init; }

    [JsonPropertyName("imdbId")]
    public string? ImdbId { get; init; }

    [JsonPropertyName("overview")]
    public string? Overview { get; init; }

    [JsonPropertyName("genres")]
    public List<string>? Genres { get; init; }

    [JsonPropertyName("images")]
    public List<Image>? Images { get; init; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; init; }

    [JsonPropertyName("originalLanguage")]
    public Language? OriginalLanguage { get; init; }
}