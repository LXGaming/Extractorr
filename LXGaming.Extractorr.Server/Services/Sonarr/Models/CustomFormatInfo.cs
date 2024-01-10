using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/v4.0.0.748/src/NzbDrone.Core/Notifications/Webhook/WebhookCustomFormatInfo.cs
public record CustomFormatInfo {

    [JsonPropertyName("customFormats")]
    public List<CustomFormat>? CustomFormats { get; init; }

    [JsonPropertyName("customFormatScore")]
    public int? CustomFormatScore { get; init; }
}