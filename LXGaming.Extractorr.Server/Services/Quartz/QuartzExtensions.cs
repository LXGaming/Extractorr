using Quartz;

namespace LXGaming.Extractorr.Server.Services.Quartz;

public static class QuartzExtensions {

    public static T? Get<T>(this JobDataMap jobDataMap, string key, T? defaultValue = default) {
        if (jobDataMap.TryGetValue(key, out var value)) {
            return (T) value;
        }

        return defaultValue;
    }

    public static T GetRequired<T>(this JobDataMap jobDataMap, string key) {
        var value = jobDataMap.Get<T>(key);
        if (value == null) {
            throw new InvalidOperationException($"Key '{key}' not found");
        }

        return value;
    }
}