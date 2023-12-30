using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public record Grab {

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; set; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; set; }

    [JsonPropertyName("movie")]
    public Movie? Movie { get; set; }

    [JsonPropertyName("release")]
    public Release? Release { get; set; }

    [JsonPropertyName("remoteMovie")]
    public RemoteMovie? RemoteMovie { get; set; }
}