using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/Notifications/Webhook/WebhookEventType.cs
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