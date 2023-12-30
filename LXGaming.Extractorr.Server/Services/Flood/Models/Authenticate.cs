using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public record Authenticate {

    [JsonPropertyName("level")]
    public AccessLevel Level { get; init; }

    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("username")]
    public string? Username { get; init; }
}