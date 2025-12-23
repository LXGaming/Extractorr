using System.Collections.Immutable;

namespace LXGaming.Extractorr.Server.Services.Event.Models;

public class ImportEventArgs : EventArgs {

    public required string Id { get; init; }

    public required ImmutableArray<string> Files { get; init; }

    public required bool Delete { get; init; }
}