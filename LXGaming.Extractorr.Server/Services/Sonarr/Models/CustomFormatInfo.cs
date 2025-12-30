using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Notifications/Webhook/WebhookCustomFormatInfo.cs
public record CustomFormatInfo {

    [JsonPropertyName("customFormats")]
    public ImmutableArray<CustomFormat>? CustomFormats { get; init; }

    [JsonPropertyName("customFormatScore")]
    public int? CustomFormatScore { get; init; }
}