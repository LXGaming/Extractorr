using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/Notifications/Webhook/WebhookGrabPayload.cs
public record GrabPayload : Payload {

    [JsonPropertyName("series")]
    public Series? Series { get; init; }

    [JsonPropertyName("episodes")]
    public List<Episode>? Episodes { get; init; }

    [JsonPropertyName("release")]
    public Release? Release { get; init; }

    [JsonPropertyName("downloadClient")]
    public string? DownloadClient { get; init; }

    [JsonPropertyName("downloadClientType")]
    public string? DownloadClientType { get; init; }

    [JsonPropertyName("downloadId")]
    public string? DownloadId { get; init; }

    [JsonPropertyName("customFormatInfo")]
    public CustomFormatInfo? CustomFormatInfo { get; init; }
}