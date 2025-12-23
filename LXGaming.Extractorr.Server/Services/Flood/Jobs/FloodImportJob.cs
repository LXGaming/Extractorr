using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood.Utilities;
using LXGaming.Extractorr.Server.Services.Quartz;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood.Jobs;

public class FloodImportJob(
    ExtractionService extractionService,
    ILogger<FloodImportJob> logger,
    TorrentService torrentService) : IJob {

    public const string EventKey = "event";
    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodImportJob));

    public async Task Execute(IJobExecutionContext context) {
        var eventArgs = context.MergedJobDataMap.GetRequired<ImportEventArgs>(EventKey);

        foreach (var torrentClient in torrentService.GetClients<FloodTorrentClient>()) {
            try {
                await ExecuteAsync(torrentClient, eventArgs);
            } catch (Exception ex) {
                logger.LogWarning(ex, "Encountered an error while executing {Client}", torrentClient);
            }
        }
    }

    private async Task ExecuteAsync(FloodTorrentClient torrentClient, ImportEventArgs eventArgs) {
        if (!eventArgs.Delete) {
            return;
        }

        var torrentProperties = await torrentClient.GetTorrentAsync(eventArgs.Id);
        if (torrentProperties == null) {
            logger.LogWarning("Invalid Import: {Id} was not found on {Client}", eventArgs.Id, torrentClient);
            return;
        }

        string torrentPath;
        try {
            torrentPath = torrentProperties.GetPath();
        } catch (Exception ex) {
            logger.LogWarning("Invalid Torrent: {Message}", ex.Message);
            return;
        }

        List<string> torrentFiles;
        try {
            torrentFiles = await torrentClient.GetTorrentFilesAsync(torrentProperties);
        } catch (Exception ex) {
            logger.LogError(ex, "Encountered an error while getting torrent files for {Name} ({Id})",
                torrentProperties.Name, torrentProperties.Hash);
            return;
        }

        if (!torrentFiles.Any(extractionService.IsExtractable)) {
            logger.LogWarning("Invalid Torrent: {Name} ({Id}) has no extractable contents", torrentProperties.Name,
                torrentProperties.Hash);
            return;
        }

        foreach (var file in eventArgs.Files) {
            var filePath = Path.GetFullPath(file);
            if (!filePath.StartsWith(torrentPath)) {
                logger.LogWarning("Invalid Import File: {File} is not inside {Path}", filePath, torrentPath);
                continue;
            }

            if (!File.Exists(filePath)) {
                logger.LogWarning("Invalid Import File: {File} does not exist", filePath);
                continue;
            }

            if (torrentFiles.Any(torrentFile => string.Equals(torrentFile, filePath, StringComparison.OrdinalIgnoreCase))) {
                logger.LogWarning("Invalid Import File: {File} is part of the torrent", filePath);
                continue;
            }

            logger.LogInformation("Deleting Import File: {File}", filePath);
            File.Delete(filePath);
        }
    }
}