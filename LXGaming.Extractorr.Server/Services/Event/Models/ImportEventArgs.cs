namespace LXGaming.Extractorr.Server.Services.Event.Models;

public class ImportEventArgs : EventArgs {

    public string Id { get; init; } = string.Empty;

    public List<string> Files { get; init; } = new();

    public bool Delete { get; init; }
}