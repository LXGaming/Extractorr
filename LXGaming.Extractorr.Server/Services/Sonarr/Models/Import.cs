using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public class Import {

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; set; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; set; }

    [JsonPropertyName("deletedFiles")]
    public List<EpisodeFile>? DeletedFiles { get; set; }

    [JsonPropertyName("episodes")]
    public List<Episode>? Episodes { get; set; }

    [JsonPropertyName("episodeFile")]
    public EpisodeFile? EpisodeFile { get; set; }

    [JsonPropertyName("isUpgrade")]
    public bool IsUpgrade { get; set; }

    [JsonPropertyName("series")]
    public Series? Series { get; set; }
}