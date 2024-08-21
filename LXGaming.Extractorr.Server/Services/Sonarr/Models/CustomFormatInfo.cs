using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/Notifications/Webhook/WebhookCustomFormatInfo.cs
public record CustomFormatInfo {

    [JsonPropertyName("customFormats")]
    public List<CustomFormat>? CustomFormats { get; init; }

    [JsonPropertyName("customFormatScore")]
    public int? CustomFormatScore { get; init; }
}