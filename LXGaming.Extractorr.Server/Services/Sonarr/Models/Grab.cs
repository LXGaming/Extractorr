using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public class Grab {

    [JsonPropertyName("DownloadClient")]
    public string? DownloadClient { get; set; }

    [JsonPropertyName("DownloadId")]
    public string? DownloadId { get; set; }

    [JsonPropertyName("Episodes")]
    public List<Episode>? Episodes { get; set; }

    [JsonPropertyName("Release")]
    public Release? Release { get; set; }

    [JsonPropertyName("Series")]
    public Series? Series { get; set; }
}