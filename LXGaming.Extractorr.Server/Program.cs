using System.IO.Compression;
using System.Net;
using LXGaming.Common.Serilog;
using LXGaming.Extractorr.Server.Services.Web.Utilities;
using LXGaming.Hosting.Generated;
using Microsoft.AspNetCore.HttpOverrides;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File.Archive;
using IPNetwork = System.Net.IPNetwork;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(new EnvironmentLoggingLevelSwitch(LogEventLevel.Verbose, LogEventLevel.Debug))
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Quartz", LogEventLevel.Information)
    .MinimumLevel.Override("Quartz.Core.ErrorLogger", LogEventLevel.Fatal)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine("logs", "server-.log"),
        buffered: true,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 1,
        hooks: new ArchiveHooks(CompressionLevel.Optimal))
    .CreateBootstrapLogger();

Log.Information("Initialising...");

try {
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddEnvironmentVariables();
    builder.Host.UseSerilog();

    builder.Services.AddControllers();

    builder.Services.Configure<ForwardedHeadersOptions>(options => {
        var section = builder.Configuration.GetSection("ForwardedHeaders");
        options.ForwardedHeaders = section.GetValue<ForwardedHeaders>("ForwardedHeaders");
        options.ForwardLimit = section.GetValue<int>("ForwardLimit");

        foreach (var value in section.GetSection("KnownProxies").Get<string[]>() ?? []) {
            options.KnownProxies.Add(IPAddress.Parse(value));
        }

        foreach (var value in section.GetSection("KnownIPNetworks").Get<string[]>() ?? []) {
            options.KnownIPNetworks.Add(IPNetwork.Parse(value));
        }
    });

    builder.Services.AddHealthChecks();

    builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));
    builder.Services.AddQuartz();
    builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    builder.Services.AddRouting(options => options.LowercaseUrls = true);

    builder.Services.AddWebService();
    builder.Services.AddAllServices();

    var app = builder.Build();

    app.UseForwardedHeaders();

    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();

    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health").RequireHost("127.0.0.1");

    await app.RunAsync();
    return 0;
} catch (Exception ex) {
    Log.Fatal(ex, "Application failed to initialise");
    return 1;
} finally {
    await Log.CloseAndFlushAsync();
}