namespace LXGaming.Extractorr.Server.Services.Flood;

public class FloodOptions {

    public const string Key = "Flood";

    public string? Address { get; init; }

    public string? Username { get; init; }

    public string? Password { get; init; }

    public string? Schedule { get; init; }

    public bool SkipActiveExtraction { get; init; }
}