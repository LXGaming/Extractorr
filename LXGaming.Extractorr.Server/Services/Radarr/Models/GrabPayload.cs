using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/4c007291833246d3ed78e6f396fc7e60cc9ca70c/src/NzbDrone.Core/Notifications/Webhook/WebhookGrabPayload.cs
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