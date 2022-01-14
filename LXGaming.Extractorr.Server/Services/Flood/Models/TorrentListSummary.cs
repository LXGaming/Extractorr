using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public class TorrentListSummary {

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("torrents")]
    public Dictionary<string, TorrentProperties>? Torrents { get; set; }
}