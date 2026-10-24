using System.Net;
using System.Text.Json;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.Web;

public class WebService(IConfiguration configuration, JsonSerializerOptions jsonSerializerOptions) {

    public JsonSerializerOptions JsonSerializerOptions { get; } = jsonSerializerOptions;

    private readonly WebOptions _options = configuration.GetSection(WebOptions.Key).Get<WebOptions>()
                                           ?? new WebOptions();

    public virtual HttpClient CreateClient(HttpMessageHandler handler) {
        var client = new HttpClient(handler);
        try {
            client.DefaultRequestHeaders.Add("User-Agent", Constants.Application.UserAgent);
            client.Timeout = TimeSpan.FromMilliseconds(_options.Timeout);
        } catch (Exception) {
            client.Dispose();
            throw;
        }

        return client;
    }

    public SocketsHttpHandler CreateHandler() {
        var handler = new SocketsHttpHandler();
        try {
            // CVE-2026-111497 (Brotli out-of-bounds write)
            handler.AutomaticDecompression = DecompressionMethods.All & ~DecompressionMethods.Brotli;
            handler.PooledConnectionLifetime = TimeSpan.FromMilliseconds(_options.PooledConnectionLifetime);
            handler.UseCookies = false;
        } catch (Exception) {
            handler.Dispose();
            throw;
        }

        return handler;
    }

    public virtual async Task<T> DeserializeAsync<T>(HttpResponseMessage response,
        CancellationToken cancellationToken = default) {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions, cancellationToken)
               ?? throw new JsonException($"Failed to deserialize {typeof(T).Name}.");
    }
}