using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

// https://github.com/jesec/flood/blob/77f4bc7267331f2c731c47dd62b570d4f0bf0c1d/shared/types/Torrent.ts#L60
public record TorrentListSummary {

    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("torrents")]
    public required Dictionary<string, TorrentProperties> Torrents { get; init; }
}