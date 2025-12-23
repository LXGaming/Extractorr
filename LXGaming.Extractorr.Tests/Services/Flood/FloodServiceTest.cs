using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Flood;
using LXGaming.Extractorr.Server.Services.Torrent;
using LXGaming.Extractorr.Server.Services.Torrent.Client;
using LXGaming.Extractorr.Server.Services.Torrent.Utilities;
using LXGaming.Extractorr.Tests.Utilities;
using LXGaming.Hosting.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LXGaming.Extractorr.Tests.Services.Flood;

[Parallelizable]
public class FloodServiceTest : ServiceTestBase {

    private FloodTorrentClient? _torrentClient;

    public FloodServiceTest() {
        Services.AddSingleton<FloodService>();
        Services.AddSingleton<ITorrentClientProvider>(provider => provider.GetRequiredService<FloodService>());
        Services.AddService<EventService>();
        Services.AddService<TorrentService>();
        Services.AddLogging();
        Services.AddSchedulerFactory();
        Services.AddWebService();
    }

    [OneTimeSetUp]
    public void Setup() {
        var client = Provider.GetRequiredService<TorrentService>().GetClient<FloodTorrentClient>();
        if (client == null) {
            Assert.Ignore("Flood torrent client has not been configured");
        }

        _torrentClient = client;
    }

    [OneTimeTearDown]
    public void Teardown() {
        _torrentClient?.Dispose();
    }

    [Test]
    public Task DeserializeTorrentsAsync() => _torrentClient!.GetTorrentsAsync();

    [Test]
    public async Task DeserializeTorrentContentsAsync() {
        var torrentListSummary = await _torrentClient!.GetTorrentsAsync();
        foreach (var pair in torrentListSummary.Torrents) {
            await _torrentClient!.GetTorrentContentsAsync(pair.Key);
        }
    }
}