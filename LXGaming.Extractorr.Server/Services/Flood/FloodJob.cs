using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Services.Quartz;
using LXGaming.Extractorr.Server.Utilities;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood;

[DisallowConcurrentExecution, PersistJobDataAfterExecution]
public class FloodJob : IJob {

    public const string TorrentsKey = "torrents";
    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodJob));
    private readonly ExtractionService _extractionService;
    private readonly FloodService _floodService;
    private readonly ILogger<FloodJob> _logger;

    public FloodJob(ExtractionService extractionService, FloodService floodService, ILogger<FloodJob> logger) {
        _extractionService = extractionService;
        _floodService = floodService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context) {
        var torrentListSummary = await _floodService.EnsureAuthenticatedAsync(_floodService.GetTorrentsAsync());
        if (torrentListSummary?.Torrents == null || torrentListSummary.Torrents.Count == 0) {
            return;
        }

        var excludedTorrents = context.TryGetOrCreateValue<HashSet<string>>(TorrentsKey);
        excludedTorrents.RemoveWhere(key => !torrentListSummary.Torrents.ContainsKey(key));

        if (_floodService.Options.SkipActiveExtraction) {
            var activeTorrents = torrentListSummary.Torrents
                .Where(pair => pair.Value.Status != null
                               && pair.Value.Status.Contains(TorrentStatus.Active)
                               && !pair.Value.Status.Contains(TorrentStatus.Complete)
                               && pair.Value.Status.Contains(TorrentStatus.Downloading))
                .ToList();
            if (activeTorrents.Count != 0) {
                _logger.LogDebug("Skipping extraction due to the following torrents:");
                foreach (var (key, value) in activeTorrents) {
                    _logger.LogDebug("- {Name} ({Id})", value.Name, key);
                }

                return;
            }
        }

        foreach (var (key, value) in torrentListSummary.Torrents) {
            if (excludedTorrents.Contains(key) || string.IsNullOrEmpty(value.Directory) || value.Status == null || value.Tags == null) {
                continue;
            }

            if (!value.Status.Contains(TorrentStatus.Complete) || !value.Tags.Contains(Constants.Application.Id)) {
                continue;
            }

            var torrentFiles = await _floodService.GetTorrentFilesAsync(value);
            if (torrentFiles.Any(_extractionService.IsExtractable)) {
                _logger.LogDebug("Processing {Name} ({Id}) for extraction", value.Name, key);
                if (!_extractionService.Execute(value.Directory, torrentFiles)) {
                    excludedTorrents.Add(key);
                    continue;
                }
            } else {
                _logger.LogWarning("Skipping {Name} ({Id}) due to no extractable contents", value.Name, key);
            }

            value.Tags.Remove(Constants.Application.Id);
            _logger.LogDebug("Setting {Name} ({Id}) Tags: {Tags}", value.Name, key, string.Join(", ", value.Tags));
            await _floodService.SetTorrentTagsAsync(new SetTorrentsTagsOptions {
                Hashes = new List<string> { key },
                Tags = value.Tags
            });
        }
    }
}