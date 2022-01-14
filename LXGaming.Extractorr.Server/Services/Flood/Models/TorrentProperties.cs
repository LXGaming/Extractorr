using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public class TorrentProperties {

    [JsonPropertyName("bytesDone")]
    public long BytesDone { get; set; }

    [JsonPropertyName("dateActive")]
    public long DateActive { get; set; }

    [JsonPropertyName("dateAdded")]
    public long DateAdded { get; set; }

    [JsonPropertyName("dateCreated")]
    public long DateCreated { get; set; }

    [JsonPropertyName("dateFinished")]
    public long DateFinished { get; set; }

    [JsonPropertyName("directory")]
    public string? Directory { get; set; }

    [JsonPropertyName("downRate")]
    public long DownRate { get; set; }

    [JsonPropertyName("downTotal")]
    public long DownTotal { get; set; }

    [JsonPropertyName("eta")]
    public decimal Eta { get; set; }

    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    [JsonPropertyName("isInitialSeeding")]
    public bool IsInitialSeeding { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("isSequential")]
    public bool IsSequential { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("peersConnected")]
    public long PeersConnected { get; set; }

    [JsonPropertyName("peersTotal")]
    public long PeersTotal { get; set; }

    [JsonPropertyName("percentComplete")]
    public decimal PercentComplete { get; set; }

    [JsonPropertyName("priority")]
    public TorrentPriority Priority { get; set; }

    [JsonPropertyName("ratio")]
    public decimal Ratio { get; set; }

    [JsonPropertyName("seedsConnected")]
    public long SeedsConnected { get; set; }

    [JsonPropertyName("seedsTotal")]
    public long SeedsTotal { get; set; }

    [JsonPropertyName("sizeBytes")]
    public long SizeBytes { get; set; }

    [JsonPropertyName("status")]
    public List<TorrentStatus>? Status { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("trackerURIs")]
    public List<string>? TrackerUris { get; set; }

    [JsonPropertyName("upRate")]
    public long UpRate { get; set; }

    [JsonPropertyName("upTotal")]
    public long UpTotal { get; set; }
}