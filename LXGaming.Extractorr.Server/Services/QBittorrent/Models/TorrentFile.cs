using System.Text.Json.Serialization;
using LXGaming.Extractorr.Server.Utilities.Json.Converters;

namespace LXGaming.Extractorr.Server.Services.QBittorrent.Models;

public record TorrentFile {

    [JsonPropertyName("availability")]
    public double Availability { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("is_seed")]
    public bool? IsSeed { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("piece_range")]
    [JsonConverter(typeof(PieceRangeConverter))]
    public required PieceRange PieceRange { get; init; }

    [JsonPropertyName("priority")]
    public DownloadPriority Priority { get; init; }

    [JsonPropertyName("progress")]
    public double Progress { get; init; }

    [JsonPropertyName("size")]
    public long Size { get; init; }
}