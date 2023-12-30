﻿using System.Text.Json.Serialization;

namespace LXGaming.Extractorr.Server.Services.Flood.Models;

public record TorrentListSummary {

    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("torrents")]
    public Dictionary<string, TorrentProperties>? Torrents { get; init; }
}