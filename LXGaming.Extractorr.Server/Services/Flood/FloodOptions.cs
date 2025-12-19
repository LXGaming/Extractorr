namespace LXGaming.Extractorr.Server.Services.Flood;

public class FloodOptions {

    public const string Key = "Flood";

    public string Address { get; set; } = "";

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public string Schedule { get; set; } = "";

    public bool SkipActiveExtraction { get; set; }
}