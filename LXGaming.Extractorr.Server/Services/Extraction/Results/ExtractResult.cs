using System.Collections.Frozen;

namespace LXGaming.Extractorr.Server.Services.Extraction.Results;

public class ExtractResult {

    public bool IsSuccess => Exception == null;

    public Exception? Exception { get; }

    public FrozenSet<string>? Volumes { get; }

    private ExtractResult(Exception? exception, FrozenSet<string>? volumes) {
        Exception = exception;
        Volumes = volumes;
    }

    public static ExtractResult FromSuccess(FrozenSet<string> volumes) {
        return new ExtractResult(null, volumes);
    }

    public static ExtractResult FromError(Exception exception, FrozenSet<string>? volumes = null) {
        return new ExtractResult(exception, volumes);
    }
}