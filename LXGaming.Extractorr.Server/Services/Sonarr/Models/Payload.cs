using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/52972e7efcce800560cbbaa64f5f76aaef6cbe77/src/NzbDrone.Core/Notifications/Webhook/WebhookPayload.cs
public abstract record Payload {

    [JsonPropertyName("eventType")]
    public EventType? EventType { get; init; }

    [JsonPropertyName("instanceName")]
    public string? InstanceName { get; init; }

    [JsonPropertyName("applicationUrl")]
    public string? ApplicationUrl { get; init; }
}