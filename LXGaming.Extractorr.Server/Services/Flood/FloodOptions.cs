namespace LXGaming.Extractorr.Server.Services.Flood;

public sealed class FloodOptions {

    public const string Key = "Flood";

    public string Schedule { get; set; } = "";

    public bool RunOnStart { get; set; }
}