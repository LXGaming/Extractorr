using System.Text.Json;
using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Radarr.Models;
using LXGaming.Extractorr.Server.Utilities;

namespace LXGaming.Extractorr.Server.Services.Radarr;

[Service(ServiceLifetime.Singleton)]
public class RadarrService(
    IConfiguration configuration,
    EventService eventService,
    ILogger<RadarrService> logger) : IHostedService {

    public readonly RadarrOptions Options = configuration.GetSection(RadarrOptions.Key).Get<RadarrOptions>()
                                            ?? throw new InvalidOperationException("RadarrOptions is unavailable");

    public Task StartAsync(CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(Options.Username) || string.IsNullOrEmpty(Options.Password)) {
            logger.LogWarning("Radarr credentials have not been configured");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(JsonDocument document) {
        if (Options.DebugWebhooks) {
            logger.LogDebug("{Content}", JsonSerializer.Serialize(document));
        }

        var eventType = document.RootElement.GetProperty("eventType").Deserialize<EventType>();

        logger.LogDebug("Processing {EventType} for Radarr", eventType);

        return eventType switch {
            EventType.Download => OnImportAsync(document.Deserialize<Import>()),
            EventType.Grab => OnGrabAsync(document.Deserialize<Grab>()),
            EventType.Test => OnTestAsync(document.Deserialize<Grab>()),
            _ => OnUnknownAsync(eventType)
        };
    }

    private Task OnGrabAsync(Grab? grab) {
        ArgumentNullException.ThrowIfNull(grab, nameof(grab));

        if (string.IsNullOrEmpty(grab.DownloadId)) {
            logger.LogWarning("Invalid Grab: Missing DownloadId");
            return Task.CompletedTask;
        }

        logger.LogInformation("Grab {DownloadId}", grab.DownloadId);
        return eventService.OnGrabAsync(grab.DownloadId);
    }

    private Task OnImportAsync(Import? import) {
        ArgumentNullException.ThrowIfNull(import, nameof(import));

        if (string.IsNullOrEmpty(import.DownloadId)) {
            logger.LogWarning("Invalid Import: Missing DownloadId");
            return Task.CompletedTask;
        }

        if (import.MovieFile == null || string.IsNullOrEmpty(import.MovieFile.Path)) {
            logger.LogWarning("Invalid Import: Missing MovieFile");
            return Task.CompletedTask;
        }

        var path = Options.RemotePathMappings != null
            ? Toolbox.GetMappedPath(Options.RemotePathMappings, import.MovieFile.Path)
            : import.MovieFile.Path;
        if (!import.MovieFile.Path.Equals(path)) {
            logger.LogInformation("Mapped {Remote} -> {Local}", import.MovieFile.Path, path);
        }

        logger.LogInformation("Import {File} ({DownloadId})", path, import.DownloadId);
        return eventService.OnImportAsync(import.DownloadId, path, Options.DeleteOnImport);
    }

    private Task OnTestAsync(Grab? grab) {
        logger.LogInformation("Test Successful");
        return Task.CompletedTask;
    }

    private Task OnUnknownAsync(EventType eventType) {
        logger.LogWarning("Unhandled Event Type: {EventType}", eventType);
        return Task.CompletedTask;
    }
}