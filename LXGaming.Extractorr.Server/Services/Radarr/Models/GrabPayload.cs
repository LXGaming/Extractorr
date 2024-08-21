using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/fc4f4ab21125cd3817133434acc0c10fba680930/src/NzbDrone.Core/Notifications/Webhook/WebhookGrabPayload.cs
public record GrabPayload : Payload {

    [JsonPropertyName("movie")]
    public Movie? Movie { get; init; }

    [JsonPropertyName("remoteMovie")]
    public RemoteMovie? RemoteMovie { get; init; }

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