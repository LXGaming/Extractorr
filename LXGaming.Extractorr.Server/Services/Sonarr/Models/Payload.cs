using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Sonarr.Models;

// https://github.com/Sonarr/Sonarr/blob/1aaa9a14bc2d64cdc0d9eaac2d303b240fd2d6ea/src/NzbDrone.Core/Notifications/Webhook/WebhookPayload.cs
public abstract record Payload {

    [JsonPropertyName("eventType")]
    public EventType? EventType { get; init; }

    [JsonPropertyName("instanceName")]
    public string? InstanceName { get; init; }

    [JsonPropertyName("applicationUrl")]
    public string? ApplicationUrl { get; init; }
}