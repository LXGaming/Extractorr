using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public class Authenticate {

    [JsonPropertyName("level")]
    public AccessLevel Level { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}