using System.Reflection;
using LXGaming.Extractorr.Server.Services.Web;
using LXGaming.Extractorr.Server.Services.Web.Utilities;
using LXGaming.Extractorr.Tests.Services.Quartz;
using LXGaming.Extractorr.Tests.Services.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace LXGaming.Extractorr.Tests.Utilities;

public static class Extensions {

    public static IServiceCollection AddConfiguration(this IServiceCollection services) {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(IConfiguration))) {
            throw new InvalidOperationException("Configuration is already registered");
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        return services.AddSingleton<IConfiguration>(configuration);
    }

    public static IServiceCollection AddSchedulerFactory(this IServiceCollection services) {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(ISchedulerFactory))) {
            throw new InvalidOperationException("SchedulerFactory is already registered");
        }

        return services.AddSingleton<ISchedulerFactory, TestSchedulerFactory>();
    }

    public static IServiceCollection AddWebService(this IServiceCollection services) {
        if (services.Any(descriptor => descriptor.ServiceType == typeof(WebService))) {
            throw new InvalidOperationException("WebService is already registered");
        }

        return services
            .AddConfiguration()
            .AddWebService<WebService, TestWebService>();
    }
}