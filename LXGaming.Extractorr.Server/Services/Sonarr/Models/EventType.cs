using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Notifications/Webhook/WebhookEventType.cs
[JsonConverter(typeof(StringEnumConverter<EventType>))]
public enum EventType {

    [JsonPropertyName("Test")]
    Test,

    [JsonPropertyName("Grab")]
    Grab,

    [JsonPropertyName("Download")]
    Download,

    [JsonPropertyName("Rename")]
    Rename,

    [JsonPropertyName("SeriesAdd")]
    SeriesAdd,

    [JsonPropertyName("SeriesDelete")]
    SeriesDelete,

    [JsonPropertyName("EpisodeFileDelete")]
    EpisodeFileDelete,

    [JsonPropertyName("Health")]
    Health,

    [JsonPropertyName("ApplicationUpdate")]
    ApplicationUpdate,

    [JsonPropertyName("HealthRestored")]
    HealthRestored,

    [JsonPropertyName("ManualInteractionRequired")]
    ManualInteractionRequired
}