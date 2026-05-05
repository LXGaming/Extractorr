using LXGaming.Extractorr.Server.Services.Torrent.Client;

namespace LXGaming.Extractorr.Server.Services.QBittorrent;

public class QBittorrentTorrentClientOptions : TorrentClientOptions {

    public string ApiKey { get; set; } = "";

    public bool UsingApiKey => !string.IsNullOrWhiteSpace(ApiKey);
}