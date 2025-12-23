using System.Net;
using LXGaming.Extractorr.Server.Services.Web;

namespace LXGaming.Extractorr.Server.Services.Torrent.Client;

public abstract class TorrentClientBase : ITorrentClient {

    public HashSet<string> ExcludedTorrents { get; } = [];

    public bool SkipActiveExtraction => Options.SkipActiveExtraction;

    protected virtual TorrentClientOptions Options { get; }

    protected ILogger Logger { get; }

    protected WebService WebService { get; }

    protected HttpClient HttpClient { get; }

    private bool _disposed;

    protected TorrentClientBase(TorrentClientOptions options, IServiceProvider serviceProvider) {
        Options = options;
        Logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
        WebService = serviceProvider.GetRequiredService<WebService>();
        HttpClient = CreateClient();
    }

    private HttpClient CreateClient() {
        if (string.IsNullOrEmpty(Options.Address)) {
            throw new InvalidOperationException("Address has not been configured");
        }

        var baseAddress = new Uri(Options.Address, UriKind.Absolute);

        var handler = WebService.CreateHandler();
        handler.CookieContainer = new CookieContainer();
        handler.UseCookies = true;

        var client = WebService.CreateClient(handler);
        client.BaseAddress = baseAddress;

        foreach (var pair in Options.AdditionalHeaders) {
            if (!client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, pair.Value)) {
                Logger.LogWarning("Failed to add {Header} header to {Name}", pair.Key, GetType().Name);
            }
        }

        return client;
    }

    public override string ToString() {
        return Options.ToString();
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        _disposed = true;

        if (disposing) {
            HttpClient.Dispose();
        }
    }
}