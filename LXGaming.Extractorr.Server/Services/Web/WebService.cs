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
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Constants.Application.UserAgent);
        client.Timeout = TimeSpan.FromMilliseconds(_options.Timeout);
        return client;
    }

    public SocketsHttpHandler CreateHandler() {
        return new SocketsHttpHandler {
            AutomaticDecompression = DecompressionMethods.All,
            PooledConnectionLifetime = TimeSpan.FromMilliseconds(_options.PooledConnectionLifetime),
            UseCookies = false
        };
    }

    public virtual async Task<T> DeserializeAsync<T>(HttpResponseMessage response,
        CancellationToken cancellationToken = default) {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions, cancellationToken)
               ?? throw new JsonException($"Failed to deserialize {nameof(T)}");
    }
}