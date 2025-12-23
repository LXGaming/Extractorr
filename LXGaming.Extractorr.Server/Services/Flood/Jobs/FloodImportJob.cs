using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood.Utilities;
using LXGaming.Extractorr.Server.Services.Quartz;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using LXGaming.Extractorr.Server.Utilities;
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
        if (torrentProperties == null || string.IsNullOrEmpty(torrentProperties.Directory)) {
            logger.LogWarning("Invalid Import: {Id} was not found on {Client}", eventArgs.Id, torrentClient);
            return;
        }

        var absoluteDirectoryPath = PathUtils.GetFullDirectoryPath(torrentProperties.Directory);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            logger.LogWarning("Invalid Torrent: {Directory} does not exist", absoluteDirectoryPath);
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