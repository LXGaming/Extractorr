using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class Import {

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; set; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; set; }

    [JsonPropertyName("deletedFiles")]
    public List<MovieFile>? DeletedFiles { get; set; }

    [JsonPropertyName("isUpgrade")]
    public bool IsUpgrade { get; set; }

    [JsonPropertyName("movie")]
    public Movie? Movie { get; set; }

    [JsonPropertyName("movieFile")]
    public MovieFile? MovieFile { get; set; }

    [JsonPropertyName("remoteMovie")]
    public RemoteMovie? RemoteMovie { get; set; }
}