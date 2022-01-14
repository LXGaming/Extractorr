using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class Import {

    [JsonPropertyName("DownloadClient")]
    public string? DownloadClient { get; set; }

    [JsonPropertyName("DownloadId")]
    public string? DownloadId { get; set; }

    [JsonPropertyName("DeletedFiles")]
    public List<MovieFile>? DeletedFiles { get; set; }

    [JsonPropertyName("IsUpgrade")]
    public bool IsUpgrade { get; set; }

    [JsonPropertyName("Movie")]
    public Movie? Movie { get; set; }

    [JsonPropertyName("MovieFile")]
    public MovieFile? MovieFile { get; set; }

    [JsonPropertyName("RemoteMovie")]
    public RemoteMovie? RemoteMovie { get; set; }
}