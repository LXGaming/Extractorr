using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/4c007291833246d3ed78e6f396fc7e60cc9ca70c/src/NzbDrone.Core/Notifications/Webhook/WebhookCustomFormatInfo.cs
public record CustomFormatInfo {

    [JsonPropertyName("customFormats")]
    public ImmutableArray<CustomFormat>? CustomFormats { get; init; }

    [JsonPropertyName("customFormatScore")]
    public int? CustomFormatScore { get; init; }
}