using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Notifications/Webhook/WebhookImportPayload.cs
public record ImportPayload : Payload {

    [JsonPropertyName("series")]
    public Series? Series { get; init; }

    [JsonPropertyName("episodes")]
    public List<Episode>? Episodes { get; init; }

    [JsonPropertyName("episodeFile")]
    public EpisodeFile? EpisodeFile { get; init; }

    [JsonPropertyName("isUpgrade")]
    public bool? IsUpgrade { get; init; }

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; init; }

    [JsonPropertyName("downloadClientType")]
    public string? DownloadClientType { get; init; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; init; }

    [JsonPropertyName("deletedFiles")]
    public List<EpisodeFile>? DeletedFiles { get; init; }

    [JsonPropertyName("customFormatInfo")]
    public CustomFormatInfo? CustomFormatInfo { get; init; }

    [JsonPropertyName("release")]
    public GrabbedRelease? Release { get; init; }
}