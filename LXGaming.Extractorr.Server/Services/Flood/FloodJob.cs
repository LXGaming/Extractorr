using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood.Models;
using LXGaming.Extractorr.Server.Utilities;
using LXGaming.Extractorr.Server.Utilities.Quartz;
using Quartz;

namespace LXGaming.Extractorr.Server.Services.Flood;

[DisallowConcurrentExecution, PersistJobDataAfterExecution]
public class FloodJob : IJob {

    public const string TorrentsKey = "torrents";
    public static readonly JobKey JobKey = JobKey.Create(nameof(FloodJob));
    private readonly ExtractionService _extractionService;
    private readonly FloodService _floodService;
    private readonly ILogger<FloodJob> _logger;

    public FloodJob(ExtractionService extractionService, FloodService floodService,  ILogger<FloodJob> logger) {
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

        foreach (var (key, value) in torrentListSummary.Torrents) {
            if (excludedTorrents.Contains(key) || value.Status == null || value.Tags == null) {
                continue;
            }

            if (!value.Status.Contains(TorrentStatus.Complete) || !value.Tags.Contains(Constants.Application.Id)) {
                continue;
            }

            _logger.LogDebug("Processing {Name} ({Id}) for extraction", value.Name, key);
            if (!_extractionService.Execute(value.Directory)) {
                excludedTorrents.Add(key);
                continue;
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