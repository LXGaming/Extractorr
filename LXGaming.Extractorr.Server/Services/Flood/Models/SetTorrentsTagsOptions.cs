using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public record SetTorrentsTagsOptions {

    [JsonPropertyName("hashes")]
    public List<string> Hashes { get; set; } = [];

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];
}