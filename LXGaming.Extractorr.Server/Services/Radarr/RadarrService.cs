using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Radarr.Models;

namespace LXGaming.Extractorr.Server.Services.Radarr;

public class RadarrService : IHostedService {

    public readonly RadarrOptions Options;
    private readonly EventService _eventService;
    private readonly ILogger<RadarrService> _logger;

    public RadarrService(IConfiguration configuration, EventService eventService, ILogger<RadarrService> logger) {
        Options = configuration.GetSection(RadarrOptions.Key).Get<RadarrOptions>();
        _eventService = eventService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(Options.Username) || string.IsNullOrEmpty(Options.Password)) {
            _logger.LogWarning("Radarr credentials have not been configured");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(JsonDocument document) {
        var eventType = document.RootElement.GetProperty("eventType").Deserialize<EventType>();

        _logger.LogDebug("Processing {EventType} for Radarr", eventType);

        return eventType switch {
            EventType.Download => OnImportAsync(document.Deserialize<Import>()),
            EventType.Grab => OnGrabAsync(document.Deserialize<Grab>()),
            EventType.Test => OnTestAsync(document.Deserialize<Grab>()),
            _ => OnUnknownAsync(eventType)
        };
    }

    private Task OnGrabAsync(Grab? grab) {
        if (grab == null) {
            throw new ArgumentNullException(nameof(grab));
        }

        if (string.IsNullOrEmpty(grab.DownloadId)) {
            _logger.LogWarning("Invalid Grab: Missing DownloadId");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Grab {DownloadId}", grab.DownloadId);
        _eventService.OnGrab(grab.DownloadId);
        return Task.CompletedTask;
    }

    private Task OnImportAsync(Import? import) {
        if (import == null) {
            throw new ArgumentNullException(nameof(import));
        }

        if (string.IsNullOrEmpty(import.DownloadId)) {
            _logger.LogWarning("Invalid Import: Missing DownloadId");
            return Task.CompletedTask;
        }

        if (import.MovieFile == null || string.IsNullOrEmpty(import.MovieFile.Path)) {
            _logger.LogWarning("Invalid Import: Missing MovieFile");
            return Task.CompletedTask;
        }

        var files = new List<string> { import.MovieFile.Path };

        _logger.LogInformation("Import {DownloadId}: {Files}", import.DownloadId, string.Join(", ", files));
        _eventService.OnImport(import.DownloadId, new List<string> { import.MovieFile.Path }, Options.DeleteOnImport);
        return Task.CompletedTask;
    }

    private Task OnTestAsync(Grab? grab) {
        _logger.LogInformation("Test Successful");
        return Task.CompletedTask;
    }

    private Task OnUnknownAsync(EventType eventType) {
        _logger.LogWarning("Unhandled Event Type: {EventType}", eventType);
        return Task.CompletedTask;
    }
}