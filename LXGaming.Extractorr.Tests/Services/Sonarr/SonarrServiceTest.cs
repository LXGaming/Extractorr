﻿using System.Reflection;
using System.Text.Json;
using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Server.Services.Event;
using LXGaming.Extractorr.Server.Services.Sonarr;
using LXGaming.Extractorr.Tests.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LXGaming.Extractorr.Tests.Services.Sonarr;

[Parallelizable]
public class SonarrServiceTest : ServiceTestBase {

    private const string ResourcePath = "LXGaming.Extractorr.Tests.Resources.Sonarr";

    private static readonly string[] Events = [
        "download",
        "grab",
        "test"
    ];

    public SonarrServiceTest() {
        Services.AddService<SonarrService>();
        Services.AddService<EventService>();
        Services.AddLogging();
        Services.AddWebService();
    }

    [TestCaseSource(nameof(Events))]
    public async Task DeserializeWebhookAsync(string id) {
        var fileName = $"event_{id}.json";

        await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{ResourcePath}.{fileName}");
        if (stream == null) {
            Assert.Ignore($"{fileName} is unavailable");
            return;
        }

        using var document = await JsonSerializer.DeserializeAsync<JsonDocument>(stream);
        Assert.That(document, Is.Not.Null, $"Failed to deserialize {fileName}");

        await Provider.GetRequiredService<SonarrService>().OnWebhookAsync(document!);
    }
}