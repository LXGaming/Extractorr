﻿using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Server.Services.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LXGaming.Extractorr.Tests.Services.Web;

[Service(ServiceLifetime.Singleton, typeof(WebService))]
public class TestWebService(IConfiguration configuration) : WebService(configuration) {

    public override async Task StartAsync(CancellationToken cancellationToken) {
        await base.StartAsync(cancellationToken);
        JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    }

    public override async Task<T> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default) {
        var expectedNode = await base.DeserializeAsync<JsonNode>(response, cancellationToken);
        return Deserialize<T>(expectedNode);
    }

    private T Deserialize<T>(JsonNode expectedNode) {
        Assert.That(expectedNode, Is.Not.Null);

        var actualObject = expectedNode.Deserialize<T>(JsonSerializerOptions);
        Assert.That(actualObject, Is.Not.Null);

        var actualNode = JsonSerializer.SerializeToNode<T>(actualObject!, JsonSerializerOptions);
        Assert.That(actualNode, Is.Not.Null);

        Warn.Unless(actualNode!.ToJsonString(), Is.EqualTo(expectedNode.ToJsonString()));

        return actualObject!;
    }
}