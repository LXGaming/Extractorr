using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Radarr.Models;
using LXGaming.Extractorr.Server.Services.Web;
using LXGaming.Extractorr.Server.Utilities;
using LXGaming.Hosting;

namespace LXGaming.Extractorr.Server.Services.Radarr;

[Service(ServiceLifetime.Singleton)]
public class RadarrService(
    IConfiguration configuration,
    EventService eventService,
    ILogger<RadarrService> logger,
    WebService webService) : IHostedService {

    public readonly RadarrOptions Options = configuration.GetSection(RadarrOptions.Key).Get<RadarrOptions>()
                                            ?? new RadarrOptions();

    public Task StartAsync(CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(Options.Username) || string.IsNullOrEmpty(Options.Password)) {
            logger.LogWarning("Radarr credentials have not been configured");
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

        logger.LogDebug("Processing {EventType} for Radarr", eventType);

        return eventType switch {
            EventType.Download => OnImportAsync(document.Deserialize<ImportPayload>(webService.JsonSerializerOptions)),
            EventType.Grab => OnGrabAsync(document.Deserialize<GrabPayload>(webService.JsonSerializerOptions)),
            EventType.Test => OnTestAsync(document.Deserialize<GrabPayload>(webService.JsonSerializerOptions)),
            _ => OnUnknownAsync(document.Deserialize<Payload>(webService.JsonSerializerOptions))
        };
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

        if (payload.MovieFile == null || string.IsNullOrEmpty(payload.MovieFile.Path)) {
            logger.LogWarning("Invalid Import: Missing MovieFile");
            return Task.CompletedTask;
        }

        var path = PathUtils.GetMappedPath(Options.RemotePathMappings, payload.MovieFile.Path);
        if (!payload.MovieFile.Path.Equals(path)) {
            logger.LogInformation("Mapped {Remote} to {Local}", payload.MovieFile.Path, path);
        }

        logger.LogInformation("Import {File} ({DownloadId})", path, payload.DownloadId);
        return eventService.OnImportAsync(payload.DownloadId, path, Options.DeleteOnImport);
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