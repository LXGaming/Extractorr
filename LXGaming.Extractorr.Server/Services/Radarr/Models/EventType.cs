using System.Text.Json.Serialization;
using LXGaming.Common.Text.Json.Serialization.Converters;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/4c007291833246d3ed78e6f396fc7e60cc9ca70c/src/NzbDrone.Core/Notifications/Webhook/WebhookEventType.cs
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

    [JsonPropertyName("MovieDelete")]
    MovieDelete,

    [JsonPropertyName("MovieFileDelete")]
    MovieFileDelete,

    [JsonPropertyName("Health")]
    Health,

    [JsonPropertyName("ApplicationUpdate")]
    ApplicationUpdate,

    [JsonPropertyName("MovieAdded")]
    MovieAdded,

    [JsonPropertyName("HealthRestored")]
    HealthRestored,

    [JsonPropertyName("ManualInteractionRequired")]
    ManualInteractionRequired
}