using Quartz;

namespace LXGaming.Extractorr.Server.Services.Quartz;

public static class QuartzExtensions {

    public static T TryGetOrCreateValue<T>(this IJobExecutionContext context, string key) where T : new() {
        if (context.JobDetail.JobDataMap.TryGetValue(key, out var existingValue)) {
            return (T) existingValue;
        }

        var value = new T();
        context.JobDetail.JobDataMap.Put(key, value);
        return value;
    }
}