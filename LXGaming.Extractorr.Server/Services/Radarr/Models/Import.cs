using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public record Import {

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; init; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; init; }

    [JsonPropertyName("deletedFiles")]
    public List<MovieFile>? DeletedFiles { get; init; }

    [JsonPropertyName("isUpgrade")]
    public bool IsUpgrade { get; init; }

    [JsonPropertyName("movie")]
    public Movie? Movie { get; init; }

    [JsonPropertyName("movieFile")]
    public MovieFile? MovieFile { get; init; }

    [JsonPropertyName("remoteMovie")]
    public RemoteMovie? RemoteMovie { get; init; }
}