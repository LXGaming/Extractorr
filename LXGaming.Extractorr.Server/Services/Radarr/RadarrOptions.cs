namespace LXGaming.Extractorr.Server.Services.Radarr;

public class RadarrOptions {

    public const string Key = "Radarr";

    public string? Username { get; init; }

    public string? Password { get; init; }

    public bool DebugWebhooks { get; init; }

    public bool DeleteOnImport { get; init; }

    public Dictionary<string, string>? RemotePathMappings { get; init; }
}