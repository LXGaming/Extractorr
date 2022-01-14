using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

public class Grab {

    [JsonPropertyName("DownloadClient")]
    public string? DownloadClient { get; set; }

    [JsonPropertyName("DownloadId")]
    public string? DownloadId { get; set; }

    [JsonPropertyName("Movie")]
    public Movie? Movie { get; set; }

    [JsonPropertyName("Release")]
    public Release? Release { get; set; }

    [JsonPropertyName("RemoteMovie")]
    public RemoteMovie? RemoteMovie { get; set; }
}