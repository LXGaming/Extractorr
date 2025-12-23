using System.Collections.Immutable;
using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Sonarr.Models;
using LXGaming.Extractorr.Server.Services.Web;
using LXGaming.Extractorr.Server.Utilities;
using LXGaming.Hosting;

namespace LXGaming.Extractorr.Server.Services.Sonarr;

[Service(ServiceLifetime.Singleton)]
public class SonarrService(
    IConfiguration configuration,
    EventService eventService,
    ILogger<SonarrService> logger,
    WebService webService) : IHostedService {

    public readonly SonarrOptions Options = configuration.GetSection(SonarrOptions.Key).Get<SonarrOptions>()
                                            ?? new SonarrOptions();

    public Task StartAsync(CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(Options.Username) || string.IsNullOrEmpty(Options.Password)) {
            logger.LogWarning("Sonarr credentials have not been configured");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task OnWebhookAsync(JsonDocument document) {
        if (Options.DebugWebhooks) {
            logger.LogDebug("{Content}", JsonSerializer.Serialize(document));
        }

        var eventType = document.RootElement
            .GetProperty("eventType")
            .Deserialize<EventType>(webService.JsonSerializerOptions);

        logger.LogDebug("Processing {EventType} for Sonarr", eventType);

        return eventType switch {
            EventType.Download => OnDownloadAsync(document),
            EventType.Grab => OnGrabAsync(document.Deserialize<GrabPayload>(webService.JsonSerializerOptions)),
            EventType.Test => OnTestAsync(document.Deserialize<GrabPayload>(webService.JsonSerializerOptions)),
            _ => OnUnknownAsync(document.Deserialize<Payload>(webService.JsonSerializerOptions))
        };
    }

    private Task OnDownloadAsync(JsonDocument? document) {
        ArgumentNullException.ThrowIfNull(document);

        if (document.RootElement.TryGetProperty("episodeFile", out _)) {
            return OnImportAsync(document.Deserialize<ImportPayload>(webService.JsonSerializerOptions));
        }

        if (document.RootElement.TryGetProperty("episodeFiles", out _)) {
            return OnImportCompleteAsync(document.Deserialize<ImportCompletePayload>(webService.JsonSerializerOptions));
        }

        return OnUnknownAsync(document.Deserialize<Payload>(webService.JsonSerializerOptions));
    }

    private Task OnGrabAsync(GrabPayload? payload) {
        ArgumentNullException.ThrowIfNull(payload);

        if (string.IsNullOrEmpty(payload.DownloadId)) {
            logger.LogWarning("Invalid Grab: Missing DownloadId");
            return Task.CompletedTask;
        }

        logger.LogInformation("Grab {DownloadId}", payload.DownloadId);
        return eventService.OnGrabAsync(payload.DownloadId);
    }

    private Task OnImportAsync(ImportPayload? payload) {
        ArgumentNullException.ThrowIfNull(payload);

        if (string.IsNullOrEmpty(payload.DownloadId)) {
            logger.LogWarning("Invalid Import: Missing DownloadId");
            return Task.CompletedTask;
        }

        if (payload.EpisodeFile == null || string.IsNullOrEmpty(payload.EpisodeFile.Path)) {
            logger.LogWarning("Invalid Import: Missing EpisodeFile");
            return Task.CompletedTask;
        }

        var path = PathUtils.GetMappedPath(Options.RemotePathMappings, payload.EpisodeFile.Path);
        if (!payload.EpisodeFile.Path.Equals(path)) {
            logger.LogInformation("Mapped {Remote} to {Local}", payload.EpisodeFile.Path, path);
        }

        logger.LogInformation("Import {File} ({DownloadId})", path, payload.DownloadId);
        return eventService.OnImportAsync(payload.DownloadId, path, Options.DeleteOnImport);
    }

    private Task OnImportCompleteAsync(ImportCompletePayload? payload) {
        ArgumentNullException.ThrowIfNull(payload);

        if (string.IsNullOrEmpty(payload.DownloadId)) {
            logger.LogWarning("Invalid Import Complete: Missing DownloadId");
            return Task.CompletedTask;
        }

        if (payload.EpisodeFiles == null) {
            logger.LogWarning("Invalid Import Complete: Missing EpisodeFiles");
            return Task.CompletedTask;
        }

        var pathsBuilder = ImmutableArray.CreateBuilder<string>(payload.EpisodeFiles.Value.Length);
        foreach (var episodeFile in payload.EpisodeFiles) {
            if (string.IsNullOrEmpty(episodeFile.Path)) {
                continue;
            }

            var path = PathUtils.GetMappedPath(Options.RemotePathMappings, episodeFile.Path);
            if (!episodeFile.Path.Equals(path)) {
                logger.LogInformation("Mapped {Remote} to {Local}", episodeFile.Path, path);
            }

            pathsBuilder.Add(path);
        }

        var paths = pathsBuilder.DrainToImmutable();
        if (paths.Length == 0) {
            logger.LogWarning("Invalid Import Complete: No Paths");
            return Task.CompletedTask;
        }

        logger.LogInformation("Import Complete {Files} ({DownloadId})", string.Join(", ", paths), payload.DownloadId);
        return eventService.OnImportAsync(payload.DownloadId, paths, Options.DeleteOnImport);
    }

    private Task OnTestAsync(GrabPayload? payload) {
        ArgumentNullException.ThrowIfNull(payload);

        logger.LogInformation("Test Successful");
        return Task.CompletedTask;
    }

    private Task OnUnknownAsync(Payload? payload) {
        ArgumentNullException.ThrowIfNull(payload);

        logger.LogWarning("Unhandled Event Type: {EventType}", payload.EventType);
        return Task.CompletedTask;
    }
}