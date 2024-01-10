using LXGaming.Extractorr.Server.Services.Event.Models;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood.Jobs;

public class ImportJob(ExtractionService extractionService, FloodService floodService, ILogger<ImportJob> logger) : IJob {

    public const string EventKey = "event";
    public static readonly JobKey JobKey = JobKey.Create(nameof(ImportJob));

    public async Task Execute(IJobExecutionContext context) {
        if (context.MergedJobDataMap.Get(EventKey) is not ImportEventArgs eventArgs) {
            throw new InvalidOperationException("Unexpected Type");
        }

        if (!eventArgs.Delete) {
            return;
        }

        var torrentProperties = await floodService.EnsureAuthenticatedAsync(() => floodService.GetTorrentAsync(eventArgs.Id));
        if (torrentProperties == null || string.IsNullOrEmpty(torrentProperties.Directory)) {
            logger.LogWarning("Invalid Import: {Id} does not exist", eventArgs.Id);
            return;
        }

        var absoluteDirectoryPath = Toolbox.GetFullDirectoryPath(torrentProperties.Directory);
        if (!Directory.Exists(absoluteDirectoryPath)) {
            logger.LogWarning("Invalid Torrent: {Directory} does not exist", absoluteDirectoryPath);
            return;
        }

        var torrentFiles = await floodService.GetTorrentFilesAsync(torrentProperties);
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