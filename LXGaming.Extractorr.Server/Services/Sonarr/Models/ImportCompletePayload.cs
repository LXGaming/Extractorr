using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Notifications/Webhook/WebhookImportCompletePayload.cs
public record ImportCompletePayload : Payload {

    [JsonPropertyName("series")]
    public Series? Series { get; init; }

    [JsonPropertyName("episodes")]
    public List<Episode>? Episodes { get; init; }

    [JsonPropertyName("episodeFiles")]
    public List<EpisodeFile>? EpisodeFiles { get; init; }

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; init; }

    [JsonPropertyName("downloadClientType")]
    public string? DownloadClientType { get; init; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; init; }

    [JsonPropertyName("release")]
    public GrabbedRelease? Release { get; init; }

    [JsonPropertyName("fileCount")]
    public int FileCount { get; init; }

    [JsonPropertyName("sourcePath")]
    public string? SourcePath { get; init; }

    [JsonPropertyName("destinationPath")]
    public string? DestinationPath { get; init; }
}