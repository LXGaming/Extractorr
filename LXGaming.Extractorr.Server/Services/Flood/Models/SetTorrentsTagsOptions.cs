using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public class SetTorrentsTagsOptions {

    [JsonPropertyName("hashes")]
    public List<string> Hashes { get; set; } = new();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}