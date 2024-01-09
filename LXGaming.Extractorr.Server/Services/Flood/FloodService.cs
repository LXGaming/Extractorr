using System.Net;
using System.Text;
using System.Text.Json;
using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Services.Web;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood;

[Service(ServiceLifetime.Singleton)]
public class FloodService(
    IConfiguration configuration,
    EventService eventService,
    ExtractionService extractionService,
    ILogger<FloodService> logger,
    ISchedulerFactory schedulerFactory,
    WebService webService) : IHostedService {

    private const uint DefaultReconnectDelay = 2;
    private const uint DefaultMaximumReconnectDelay = 300; // 5 Minutes

    public readonly FloodOptions Options = configuration.GetSection(FloodOptions.Key).Get<FloodOptions>()
                                           ?? throw new InvalidOperationException("FloodOptions is unavailable");

    private HttpClient? _httpClient;

    public async Task StartAsync(CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(Options.Address)) {
            logger.LogWarning("Flood address has not been configured");
            return;
        }

        if (string.IsNullOrEmpty(Options.Username) || string.IsNullOrEmpty(Options.Password)) {
            logger.LogWarning("Flood credentials have not been configured");
            return;
        }

        _httpClient = webService.CreateHttpClient(new HttpClientHandler {
            CookieContainer = new CookieContainer(),
            UseCookies = true
        });
        _httpClient.BaseAddress = new Uri(Options.Address);

        var reconnectDelay = DefaultReconnectDelay;
        while (true) {
            try {
                var authenticate = await AuthenticateAsync();
                if (!authenticate.Success) {
                    logger.LogWarning("Flood authentication failed");
                    return;
                }

                logger.LogInformation("Connected to Flood as {Username} ({Level})", authenticate.Username, authenticate.Level);
                break;
            } catch (HttpRequestException ex) {
                if (ex is { StatusCode: HttpStatusCode.Unauthorized }) {
                    throw;
                }

                var delay = TimeSpan.FromSeconds(reconnectDelay);
                reconnectDelay = Math.Min(reconnectDelay << 1, DefaultMaximumReconnectDelay);

                logger.LogWarning("Connection failed! Next attempt in {Delay}: {Message}", delay, ex.Message);
                await Task.Delay(delay, cancellationToken);
            }
        }

        if (!string.IsNullOrEmpty(Options.Schedule)) {
            var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            await scheduler.ScheduleJob(
                JobBuilder.Create<FloodJob>().WithIdentity(FloodJob.JobKey).Build(),
                TriggerBuilder.Create().WithCronSchedule(Options.Schedule).Build(),
                cancellationToken);
        } else {
            logger.LogWarning("Flood schedule has not been configured");
        }

        eventService.Grab += OnGrabAsync;
        eventService.Import += OnImportAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _httpClient?.Dispose();
        return Task.CompletedTask;
    }

    public async Task<T> EnsureAuthenticatedAsync<T>(Func<Task<T>> task) {
        try {
            return await task();
        } catch (HttpRequestException ex) {
            if (ex is not { StatusCode: HttpStatusCode.Unauthorized }) {
                throw;
            }
        }

        var authenticate = await AuthenticateAsync();
        if (authenticate.Success) {
            logger.LogInformation("Reconnected to Flood as {Username} ({Level})", authenticate.Username, authenticate.Level);
        } else {
            logger.LogWarning("Reconnection failed!");
        }

        return await task();
    }

    public async Task<Authenticate> AuthenticateAsync() {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        if (string.IsNullOrEmpty(Options.Username) || string.IsNullOrEmpty(Options.Password)) {
            throw new InvalidOperationException("Flood credentials have not been configured");
        }

        // ReSharper disable once UsingStatementResourceInitialization
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/authenticate") {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "username", Options.Username },
                { "password", Options.Password }
            })
        };
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await webService.DeserializeAsync<Authenticate>(response);
    }

    public async Task<TorrentProperties?> GetTorrentAsync(string hash) {
        var torrentListSummary = await GetTorrentsAsync();
        if (torrentListSummary?.Torrents?.TryGetValue(hash, out var torrentProperties) ?? false) {
            return torrentProperties;
        }

        return null;
    }

    public async Task<TorrentListSummary> GetTorrentsAsync() {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/torrents");
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await webService.DeserializeAsync<TorrentListSummary>(response);
    }

    public async Task<List<string>> GetTorrentFilesAsync(TorrentProperties torrentProperties) {
        if (string.IsNullOrEmpty(torrentProperties.Directory) || string.IsNullOrEmpty(torrentProperties.Hash)) {
            throw new InvalidOperationException("Invalid TorrentProperties");
        }

        var absoluteDirectoryPath = Toolbox.GetFullDirectoryPath(torrentProperties.Directory);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            throw new InvalidOperationException($"{absoluteDirectoryPath} does not exist");
        }

        var torrentContents = await GetTorrentContentsAsync(torrentProperties.Hash);
        if (torrentContents == null || torrentContents.Count == 0) {
            throw new InvalidOperationException($"{torrentProperties.Hash} has no contents");
        }

        var files = new List<string>();
        foreach (var torrentContent in torrentContents) {
            if (string.IsNullOrEmpty(torrentContent.Path)) {
                continue;
            }

            var absolutePath = Path.GetFullPath(torrentContent.Path, absoluteDirectoryPath);
            if (!File.Exists(absolutePath)) {
                if (!Directory.Exists(absolutePath)) {
                    throw new InvalidOperationException($"{absolutePath} does not exist");
                }

                continue;
            }

            files.Add(absolutePath);
        }

        return files;
    }

    public async Task<List<TorrentContent>> GetTorrentContentsAsync(string hash) {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/torrents/{hash}/contents");
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await webService.DeserializeAsync<List<TorrentContent>>(response);
    }

    public async Task SetTorrentTagsAsync(SetTorrentsTagsOptions options) {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/torrents/tags");
        request.Content = new StringContent(JsonSerializer.Serialize(options), Encoding.UTF8, "application/json");
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
    }

    private async void OnGrabAsync(object? sender, GrabEventArgs eventArgs) {
        var torrentProperties = await EnsureAuthenticatedAsync(() => GetTorrentAsync(eventArgs.Id));
        if (torrentProperties == null || string.IsNullOrEmpty(torrentProperties.Hash)) {
            logger.LogWarning("Invalid Grab: {Id} does not exist", eventArgs.Id);
            return;
        }

        var tags = torrentProperties.Tags ?? [];
        tags.Add(Constants.Application.Id);

        logger.LogDebug("Setting {Name} ({Id}) Tags: {Tags}", torrentProperties.Name, torrentProperties.Hash, string.Join(", ", tags));
        await SetTorrentTagsAsync(new SetTorrentsTagsOptions {
            Hashes = { torrentProperties.Hash },
            Tags = tags
        });
    }

    private async void OnImportAsync(object? sender, ImportEventArgs eventArgs) {
        if (!eventArgs.Delete) {
            return;
        }

        var torrentProperties = await EnsureAuthenticatedAsync(() => GetTorrentAsync(eventArgs.Id));
        if (torrentProperties == null || string.IsNullOrEmpty(torrentProperties.Directory)) {
            logger.LogWarning("Invalid Import: {Id} does not exist", eventArgs.Id);
            return;
        }

        var absoluteDirectoryPath = Toolbox.GetFullDirectoryPath(torrentProperties.Directory);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            logger.LogWarning("Invalid Torrent: {Directory} does not exist", absoluteDirectoryPath);
            return;
        }

        var torrentFiles = await GetTorrentFilesAsync(torrentProperties);
        if (!torrentFiles.Any(extractionService.IsExtractable)) {
            logger.LogWarning("Invalid Torrent: {Id} has no extractable contents", eventArgs.Id);
            return;
        }

        foreach (var file in eventArgs.Files) {
            var absoluteFilePath = Path.GetFullPath(file);
            if (!absoluteFilePath.StartsWith(absoluteDirectoryPath)) {
                logger.LogWarning("Invalid Import File: {File}", absoluteFilePath);
                continue;
            }

            if (!File.Exists(absoluteFilePath)) {
                logger.LogWarning("Invalid Import File: {File} does not exist", absoluteFilePath);
                continue;
            }

            if (torrentFiles.Any(torrentFile => string.Equals(torrentFile, absoluteFilePath, StringComparison.OrdinalIgnoreCase))) {
                logger.LogWarning("Invalid Import File: {File} is part of the torrent", absoluteFilePath);
                continue;
            }

            logger.LogInformation("Deleting Import File: {File}", absoluteFilePath);
            File.Delete(absoluteFilePath);
        }
    }
}