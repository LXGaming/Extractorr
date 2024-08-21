using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Radarr.Models;

// https://github.com/Radarr/Radarr/blob/fc4f4ab21125cd3817133434acc0c10fba680930/src/NzbDrone.Core/Notifications/Webhook/WebhookPayload.cs
public abstract record Payload {

    [JsonPropertyName("eventType")]
    public EventType? EventType { get; init; }

    [JsonPropertyName("instanceName")]
    public string? InstanceName { get; init; }

    [JsonPropertyName("applicationUrl")]
    public string? ApplicationUrl { get; init; }
}