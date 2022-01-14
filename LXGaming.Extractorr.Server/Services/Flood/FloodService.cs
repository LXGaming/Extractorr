﻿using System.Net;
using System.Text;
using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Utilities;
using LXGaming.Extractorr.Server.Utilities.Quartz;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood;

public class FloodService : IHostedService {

    private readonly FloodOptions _options;
    private readonly EventService _eventService;
    private readonly ExtractionService _extractionService;
    private readonly ILogger<FloodService> _logger;
    private readonly ISchedulerFactory _schedulerFactory;
    private HttpClient? _httpClient;

    public FloodService(IConfiguration configuration, EventService eventService, ExtractionService extractionService, ILogger<FloodService> logger, ISchedulerFactory schedulerFactory) {
        _options = configuration.GetSection(FloodOptions.Key).Get<FloodOptions>();
        _eventService = eventService;
        _extractionService = extractionService;
        _logger = logger;
        _schedulerFactory = schedulerFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(_options.Address)) {
            _logger.LogWarning("Flood address has not been configured");
            return;
        }

        if (string.IsNullOrEmpty(_options.Username) || string.IsNullOrEmpty(_options.Password)) {
            _logger.LogWarning("Flood credentials have not been configured");
            return;
        }

        if (string.IsNullOrEmpty(_options.Schedule)) {
            _logger.LogWarning("Flood schedule has not been configured");
            return;
        }

        _httpClient = new HttpClient(new HttpClientHandler {
            CookieContainer = new CookieContainer(),
            UseCookies = true
        });
        _httpClient.BaseAddress = new Uri(_options.Address);
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Constants.Application.UserAgent);

        var authenticate = await AuthenticateAsync(_options.Username, _options.Password);
        if (authenticate is not { Success: true }) {
            _logger.LogWarning("Flood authentication failed");
            return;
        }

        _logger.LogInformation("Connected to Flood as {Username} ({Level})", authenticate.Username, authenticate.Level);

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.ScheduleJobAsync<FloodJob>(FloodJob.JobKey, TriggerBuilder.Create().WithCronSchedule(_options.Schedule).Build());

        _eventService.Grab += OnGrabAsync;
        _eventService.Import += OnImportAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _httpClient?.Dispose();
        return Task.CompletedTask;
    }

    public async Task<T?> EnsureAuthenticatedAsync<T>(Task<T> task) {
        try {
            return await task;
        } catch (HttpRequestException ex) {
            if (ex is not { StatusCode: HttpStatusCode.Forbidden }) {
                throw;
            }
        }

        var authenticate = await AuthenticateAsync(_options.Username ?? "", _options.Password ?? "");
        if (authenticate is not { Success: true }) {
            return default;
        }

        return await task;
    }

    public async Task<Authenticate?> AuthenticateAsync(string username, string password) {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/authenticate") {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                {"username", username},
                {"password", password}
            })
        };
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<Authenticate>(stream, Toolbox.JsonSerializerOptions);
    }

    public async Task<TorrentProperties?> GetTorrentAsync(string hash) {
        var torrentListSummary = await GetTorrentsAsync();
        if (torrentListSummary?.Torrents?.TryGetValue(hash, out var torrentProperties) ?? false) {
            return torrentProperties;
        }

        return null;
    }

    public async Task<TorrentListSummary?> GetTorrentsAsync() {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/torrents");
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<TorrentListSummary>(stream, Toolbox.JsonSerializerOptions);
    }

    public async Task<List<TorrentContent>?> GetTorrentContentsAsync(string hash) {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/torrents/{hash}/contents");
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<List<TorrentContent>>(stream, Toolbox.JsonSerializerOptions);
    }

    public async Task SetTorrentTagsAsync(SetTorrentsTagsOptions options) {
        if (_httpClient == null) {
            throw new InvalidOperationException("HttpClient is unavailable");
        }

        using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/torrents/tags") {
            Content = new StringContent(JsonSerializer.Serialize(options), Encoding.UTF8, "application/json")
        };
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async void OnGrabAsync(object? sender, GrabEventArgs eventArgs) {
        var torrentProperties = await GetTorrentAsync(eventArgs.Id);
        if (torrentProperties == null || string.IsNullOrEmpty(torrentProperties.Hash)) {
            _logger.LogWarning("Invalid Grab: {Id} does not exist", eventArgs.Id);
            return;
        }

        var tags = torrentProperties.Tags ?? new List<string>();
        tags.Add(Constants.Application.Id);

        _logger.LogDebug("Setting {Name} ({Id}) Tags: {Tags}", torrentProperties.Name, torrentProperties.Hash, string.Join(", ", tags));
        await SetTorrentTagsAsync(new SetTorrentsTagsOptions {
            Hashes = new List<string> { torrentProperties.Hash },
            Tags = tags
        });
    }

    private async void OnImportAsync(object? sender, ImportEventArgs eventArgs) {
        if (!eventArgs.Delete) {
            return;
        }

        var torrentProperties = await GetTorrentAsync(eventArgs.Id);
        if (torrentProperties == null || string.IsNullOrEmpty(torrentProperties.Hash) || string.IsNullOrEmpty(torrentProperties.Directory)) {
            _logger.LogWarning("Invalid Import: {Id} does not exist", eventArgs.Id);
            return;
        }

        var absoluteDirectoryPath = Toolbox.GetFullDirectoryPath(torrentProperties.Directory);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            _logger.LogWarning("Invalid Torrent: {Directory} does not exist", absoluteDirectoryPath);
            return;
        }

        var torrentContents = await GetTorrentContentsAsync(torrentProperties.Hash);
        if (torrentContents == null || torrentContents.Count == 0) {
            _logger.LogWarning("Invalid Torrent: {Id} has no contents", eventArgs.Id);
            return;
        }

        if (!torrentContents.Any(content => _extractionService.IsExtractable(content.Filename))) {
            _logger.LogWarning("Invalid Torrent: {Id} has no extractable contents", eventArgs.Id);
            return;
        }

        foreach (var file in eventArgs.Files) {
            var absoluteFilePath = Path.GetFullPath(file);
            if (!absoluteFilePath.StartsWith(absoluteDirectoryPath)) {
                _logger.LogWarning("Invalid Import File: {File}", absoluteFilePath);
                continue;
            }

            if (!File.Exists(absoluteFilePath)) {
                _logger.LogWarning("Invalid Import File: {File} does not exist", absoluteFilePath);
                continue;
            }

            var relativeFilePath = Path.GetRelativePath(absoluteDirectoryPath, absoluteFilePath);
            if (torrentContents.Any(content => string.Equals(content.Path, relativeFilePath, StringComparison.OrdinalIgnoreCase))) {
                _logger.LogWarning("Invalid Import File: {File} is part of the torrent", absoluteFilePath);
                continue;
            }

            _logger.LogInformation("Deleting Import File: {File}", absoluteFilePath);
            File.Delete(absoluteFilePath);
        }
    }
}