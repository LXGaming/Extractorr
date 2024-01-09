using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using LXGaming.Common.Hosting;
using LXGaming.Common.Text.Json;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.Web;

[Service(ServiceLifetime.Singleton)]
public class WebService(IConfiguration configuration) : IHostedService {

    public JsonSerializerOptions JsonSerializerOptions { get; private set; } = null!;
    protected WebOptions Options { get; } = configuration.GetSection(WebOptions.Key).Get<WebOptions>()
                                            ?? throw new InvalidOperationException("WebOptions is unavailable");

    public virtual Task StartAsync(CancellationToken cancellationToken) {
        JsonSerializerOptions = new JsonSerializerOptions {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                .WithOrderPropertiesModifier()
                .WithRequiredPropertiesModifier()
        };

        return Task.CompletedTask;
    }

    public virtual Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public virtual HttpClient CreateHttpClient(HttpMessageHandler handler) {
        var httpClient = new HttpClient(handler) {
            Timeout = TimeSpan.FromMilliseconds(Options.Timeout)
        };
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Constants.Application.UserAgent);
        return httpClient;
    }

    public virtual async Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default) {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions, cancellationToken)
               ?? throw new JsonException($"Failed to deserialize {nameof(T)}");
    }
}