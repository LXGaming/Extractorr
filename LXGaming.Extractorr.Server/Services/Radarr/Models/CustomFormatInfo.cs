using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/v5.2.6.8376/src/NzbDrone.Core/Notifications/Webhook/WebhookCustomFormatInfo.cs
public record CustomFormatInfo {

    [JsonPropertyName("customFormats")]
    public List<CustomFormat>? CustomFormats { get; init; }

    [JsonPropertyName("customFormatScore")]
    public int? CustomFormatScore { get; init; }
}