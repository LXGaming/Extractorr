using System.Collections.Immutable;

namespace LXGaming.Extractorr.Server.Services.Extraction.Results;

public class ExtractResult {

    public bool State { get; }

    public IReadOnlyCollection<string>? Volumes { get; }

    private ExtractResult(bool state, IReadOnlyCollection<string>? volumes) {
        State = state;
        Volumes = volumes;
    }

    public static ExtractResult FromError(IEnumerable<string> volumes) => new(false, volumes.ToImmutableHashSet());

    public static ExtractResult FromSuccess(IEnumerable<string> volumes) => new(true, volumes.ToImmutableHashSet());
}