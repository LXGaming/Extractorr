using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Services.Quartz;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood;

[DisallowConcurrentExecution]
[PersistJobDataAfterExecution]
public class FloodJob(
    ExtractionService extractionService,
    FloodService floodService,
    ILogger<FloodJob> logger) : IJob {

    public const string TorrentsKey = "torrents";
    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodJob));

    public async Task Execute(IJobExecutionContext context) {
        var torrentListSummary = await floodService.EnsureAuthenticatedAsync(floodService.GetTorrentsAsync);
        if (torrentListSummary.Torrents.Count == 0) {
            return;
        }

        var excludedTorrents = context.TryGetOrCreateValue<HashSet<string>>(TorrentsKey);
        excludedTorrents.RemoveWhere(key => !torrentListSummary.Torrents.ContainsKey(key));

        if (floodService.Options.SkipActiveExtraction) {
            var activeTorrents = torrentListSummary.Torrents
                .Where(pair => pair.Value.Status.Contains(TorrentStatus.Active)
                               && !pair.Value.Status.Contains(TorrentStatus.Complete)
                               && pair.Value.Status.Contains(TorrentStatus.Downloading))
                .ToList();
            if (activeTorrents.Count != 0) {
                logger.LogDebug("Skipping extraction due to the following torrents:");
                foreach (var (key, value) in activeTorrents) {
                    logger.LogDebug("- {Name} ({Id})", value.Name, key);
                }

                return;
            }
        }

        foreach (var (key, value) in torrentListSummary.Torrents) {
            if (excludedTorrents.Contains(key) || string.IsNullOrEmpty(value.Directory)) {
                continue;
            }

            if (!value.Status.Contains(TorrentStatus.Complete) || !value.Tags.Contains(Constants.Application.Id)) {
                continue;
            }

            var torrentFiles = await floodService.GetTorrentFilesAsync(value);
            if (torrentFiles.Any(extractionService.IsExtractable)) {
                logger.LogDebug("Processing {Name} ({Id}) for extraction", value.Name, key);
                if (!extractionService.Execute(value.Directory, torrentFiles)) {
                    excludedTorrents.Add(key);
                    continue;
                }
            } else {
                logger.LogWarning("Skipping {Name} ({Id}) due to no extractable contents", value.Name, key);
            }

            value.Tags.Remove(Constants.Application.Id);
            logger.LogDebug("Setting {Name} ({Id}) Tags: {Tags}", value.Name, key, string.Join(", ", value.Tags));
            await floodService.SetTorrentTagsAsync(new SetTorrentsTagsOptions {
                Hashes = [key],
                Tags = value.Tags
            });
        }
    }
}