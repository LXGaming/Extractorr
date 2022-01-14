using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public class Import {

    [JsonPropertyName("DownloadClient")]
    public string? DownloadClient { get; set; }

    [JsonPropertyName("DownloadId")]
    public string? DownloadId { get; set; }

    [JsonPropertyName("DeletedFiles")]
    public List<EpisodeFile>? DeletedFiles { get; set; }

    [JsonPropertyName("Episodes")]
    public List<Episode>? Episodes { get; set; }

    [JsonPropertyName("EpisodeFile")]
    public List<EpisodeFile>? EpisodeFile { get; set; }

    [JsonPropertyName("IsUpgrade")]
    public bool IsUpgrade { get; set; }

    [JsonPropertyName("Series")]
    public Series? Series { get; set; }
}