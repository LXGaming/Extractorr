using System.Text.Json;
using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Sonarr.Models;
using LXGaming.Extractorr.Server.Services.Web;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.Sonarr;

[Service(ServiceLifetime.Singleton)]
public class SonarrService(
    IConfiguration configuration,
    EventService eventService,
    ILogger<SonarrService> logger,
    WebService webService) : IHostedService {

    public readonly SonarrOptions Options = configuration.GetSection(SonarrOptions.Key).Get<SonarrOptions>()
                                            ?? throw new InvalidOperationException("SonarrOptions is unavailable");

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
            EventType.Download => OnImportAsync(document.Deserialize<ImportPayload>(webService.JsonSerializerOptions)),
            EventType.Grab => OnGrabAsync(document.Deserialize<GrabPayload>(webService.JsonSerializerOptions)),
            EventType.Test => OnTestAsync(document.Deserialize<GrabPayload>(webService.JsonSerializerOptions)),
            _ => OnUnknownAsync(document.Deserialize<Payload>(webService.JsonSerializerOptions))
        };
    }

    private Task OnGrabAsync(GrabPayload? payload) {
        ArgumentNullException.ThrowIfNull(payload, nameof(payload));

        if (string.IsNullOrEmpty(payload.DownloadId)) {
            logger.LogWarning("Invalid Grab: Missing DownloadId");
            return Task.CompletedTask;
        }

        logger.LogInformation("Grab {DownloadId}", payload.DownloadId);
        return eventService.OnGrabAsync(payload.DownloadId);
    }

    private Task OnImportAsync(ImportPayload? payload) {
        ArgumentNullException.ThrowIfNull(payload, nameof(payload));

        if (string.IsNullOrEmpty(payload.DownloadId)) {
            logger.LogWarning("Invalid Import: Missing DownloadId");
            return Task.CompletedTask;
        }

        if (payload.EpisodeFile == null || string.IsNullOrEmpty(payload.EpisodeFile.Path)) {
            logger.LogWarning("Invalid Import: Missing EpisodeFile");
            return Task.CompletedTask;
        }

        var path = Options.RemotePathMappings != null
            ? Toolbox.GetMappedPath(Options.RemotePathMappings, payload.EpisodeFile.Path)
            : payload.EpisodeFile.Path;
        if (!payload.EpisodeFile.Path.Equals(path)) {
            logger.LogInformation("Mapped {Remote} -> {Local}", payload.EpisodeFile.Path, path);
        }

        logger.LogInformation("Import {File} ({DownloadId})", path, payload.DownloadId);
        return eventService.OnImportAsync(payload.DownloadId, path, Options.DeleteOnImport);
    }

    private Task OnTestAsync(GrabPayload? payload) {
        ArgumentNullException.ThrowIfNull(payload, nameof(payload));

        logger.LogInformation("Test Successful");
        return Task.CompletedTask;
    }

    private Task OnUnknownAsync(Payload? payload) {
        ArgumentNullException.ThrowIfNull(payload, nameof(payload));

        logger.LogWarning("Unhandled Event Type: {EventType}", payload.EventType);
        return Task.CompletedTask;
    }
}