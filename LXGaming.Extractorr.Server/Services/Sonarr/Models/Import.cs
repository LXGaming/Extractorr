using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

public record Import {

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; init; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; init; }

    [JsonPropertyName("deletedFiles")]
    public List<EpisodeFile>? DeletedFiles { get; init; }

    [JsonPropertyName("episodes")]
    public List<Episode>? Episodes { get; init; }

    [JsonPropertyName("episodeFile")]
    public EpisodeFile? EpisodeFile { get; init; }

    [JsonPropertyName("isUpgrade")]
    public bool IsUpgrade { get; init; }

    [JsonPropertyName("series")]
    public Series? Series { get; init; }
}