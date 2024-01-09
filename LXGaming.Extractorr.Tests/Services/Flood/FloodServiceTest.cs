using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Extraction;
using LXGaming.Extractorr.Server.Services.Flood;
using LXGaming.Extractorr.Tests.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LXGaming.Extractorr.Tests.Services.Flood;

[Parallelizable]
public class FloodServiceTest : ServiceTestBase {

    public FloodServiceTest() {
        Services.AddService<FloodService>();
        Services.AddService<EventService>();
        Services.AddService<ExtractionService>();
        Services.AddLogging();
        Services.AddSchedulerFactory();
        Services.AddWebService();
    }

    [OneTimeSetUp]
    public void Setup() {
        var options = Provider.GetRequiredService<IConfiguration>().GetSection(FloodOptions.Key).Get<FloodOptions>();
        if (string.IsNullOrEmpty(options?.Username) || string.IsNullOrEmpty(options?.Password)) {
            Assert.Ignore("Flood credentials have not been configured");
        }
    }

    [Test]
    [Order(1)]
    public Task DeserializeAuthenticateAsync() => Provider.GetRequiredService<FloodService>().AuthenticateAsync();

    [Test]
    [Order(2)]
    public Task DeserializeTorrentsAsync() => Provider.GetRequiredService<FloodService>().GetTorrentsAsync();

    [Test]
    public async Task DeserializeTorrentContentsAsync() {
        var torrentListSummary = await Provider.GetRequiredService<FloodService>().GetTorrentsAsync();
        foreach (var pair in torrentListSummary.Torrents) {
            await Provider.GetRequiredService<FloodService>().GetTorrentContentsAsync(pair.Key);
        }
    }
}