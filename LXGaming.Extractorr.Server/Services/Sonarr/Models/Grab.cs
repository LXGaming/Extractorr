using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public class Grab {

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; set; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; set; }

    [JsonPropertyName("episodes")]
    public List<Episode>? Episodes { get; set; }

    [JsonPropertyName("release")]
    public Release? Release { get; set; }

    [JsonPropertyName("series")]
    public Series? Series { get; set; }
}