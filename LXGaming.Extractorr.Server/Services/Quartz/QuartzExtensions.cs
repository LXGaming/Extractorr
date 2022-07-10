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

    public static Task<DateTimeOffset> ScheduleJobAsync<T>(this IScheduler scheduler, ITrigger trigger) where T : IJob {
        var key = JobKey.Create(Guid.NewGuid().ToString());
        return scheduler.ScheduleJobAsync<T>(key, trigger);
    }

    public static Task<DateTimeOffset> ScheduleJobAsync<T>(this IScheduler scheduler, JobKey key, ITrigger trigger) where T : IJob {
        var jobDetail = JobBuilder.Create<T>().WithIdentity(key).Build();
        return scheduler.ScheduleJob(jobDetail, trigger);
    }
}