using System.Reflection;
using LXGaming.Common.Hosting;
using LXGaming.Extractorr.Tests.Services.Quartz;
using LXGaming.Extractorr.Tests.Services.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace LXGaming.Extractorr.Tests.Utilities;

public static class Extensions {

    public static IServiceCollection AddConfiguration(this IServiceCollection services) {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        return services.AddSingleton<IConfiguration>(configuration);
    }

    public static IServiceCollection AddSchedulerFactory(this IServiceCollection services) {
        return services.AddSingleton<ISchedulerFactory, TestSchedulerFactory>();
    }

    public static IServiceCollection AddWebService(this IServiceCollection services) {
        return services
            .AddConfiguration()
            .AddService<TestWebService>();
    }
}