using System.IO.Compression;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File.Archive;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Quartz", LogEventLevel.Information)
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

    builder.Host.UseSerilog();

    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHealthChecks();
    builder.Services.AddQuartz(configurator => configurator.UseMicrosoftDependencyInjectionJobFactory());
    builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    builder.Services.Configure<ForwardedHeadersOptions>(options => {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        builder.Configuration
            .GetSection("ForwardedHeaders:KnownProxies")
            .Get<List<string>>()
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

    app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/health").RequireHost("127.0.0.1");
    });

    app.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Application failed to initialize");
} finally {
    Log.CloseAndFlush();
}