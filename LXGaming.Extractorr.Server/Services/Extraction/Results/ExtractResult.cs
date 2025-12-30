using System.Collections.Immutable;

namespace LXGaming.Extractorr.Server.Services.Extraction.Results;

public class ExtractResult {

    public bool IsSuccess => Exception == null;

    public Exception? Exception { get; }

    public ImmutableHashSet<string>? Volumes { get; }

    private ExtractResult(Exception? exception, ImmutableHashSet<string>? volumes) {
        Exception = exception;
        Volumes = volumes;
    }

    public static ExtractResult FromSuccess(ImmutableHashSet<string> volumes) {
        return new ExtractResult(null, volumes);
    }

    public static ExtractResult FromError(Exception exception, ImmutableHashSet<string>? volumes = null) {
        return new ExtractResult(exception, volumes);
    }
}