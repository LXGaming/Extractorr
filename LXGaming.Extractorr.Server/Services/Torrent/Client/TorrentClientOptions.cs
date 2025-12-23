using LXGaming.Extractorr.Server.Services.Torrent.Models;

namespace LXGaming.Extractorr.Server.Services.Torrent.Client;

public sealed class TorrentClientOptions {

    public TorrentClientType Type { get; set; }

    public bool Enabled { get; set; }

    public string Name { get; set; } = "";

    public string Address { get; set; } = "";

    public bool BypassAuthentication { get; set; }

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public Dictionary<string, string> AdditionalHeaders { get; set; } = new();

    public bool SkipActiveExtraction { get; set; }
}