using System.IO.Compression;
using System.Net;
using System.Reflection;
using LXGaming.Common.Hosting;
using LXGaming.Common.Serilog;
using Microsoft.AspNetCore.HttpOverrides;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File.Archive;

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

Log.Information("Initializing...");

try {
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddEnvironmentVariables();
    builder.Host.UseSerilog();

    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();

    builder.Services.Configure<QuartzOptions>(builder.Configuration.GetSection("Quartz"));
    builder.Services.AddQuartz();
    builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    builder.Services.AddAllServices(Assembly.GetExecutingAssembly());

    builder.Services.Configure<ForwardedHeadersOptions>(options => {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        builder.Configuration
            .GetSection("ForwardedHeaders:KnownProxies")
            .Get<List<string>>()?
            .Select(IPAddress.Parse)
            .ToList()
            .ForEach(options.KnownProxies.Add);
    });

    var app = builder.Build();

    app.UseForwardedHeaders();

    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health").RequireHost("127.0.0.1");

    await app.RunAsync();
    return 0;
} catch (Exception ex) {
    Log.Fatal(ex, "Application failed to initialize");
    return 1;
} finally {
    Log.CloseAndFlush();
}