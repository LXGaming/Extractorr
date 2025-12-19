using System.Text.Json;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Radarr;
using LXGaming.Extractorr.Tests.Utilities;
using LXGaming.Hosting.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LXGaming.Extractorr.Tests.Services.Radarr;

[Parallelizable]
public class RadarrServiceTest : ServiceTestBase {

    private const string ResourcePath = "LXGaming.Extractorr.Tests.Resources.Radarr";

    private static readonly string[] Events = [
        "download",
        "grab",
        "test"
    ];

    public RadarrServiceTest() {
        Services.AddService<RadarrService>();
        Services.AddService<EventService>();
        Services.AddLogging();
        Services.AddWebService();
    }

    [TestCaseSource(nameof(Events))]
    public async Task DeserializeWebhookAsync(string id) {
        var fileName = $"event_{id}.json";

        await using var stream = typeof(RadarrServiceTest).Assembly.GetManifestResourceStream($"{ResourcePath}.{fileName}");
        if (stream == null) {
            Assert.Ignore($"{fileName} is unavailable");
            return;
        }

        using var document = await JsonSerializer.DeserializeAsync<JsonDocument>(stream);
        Assert.That(document, Is.Not.Null, $"Failed to deserialize {fileName}");

        await Provider.GetRequiredService<RadarrService>().OnWebhookAsync(document!);
    }
}