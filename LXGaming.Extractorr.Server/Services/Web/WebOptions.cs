namespace LXGaming.Extractorr.Server.Services.Web;

public class WebOptions {

    public const int DefaultPooledConnectionLifetime = 300000; // 5 Minutes
    public const int DefaultTimeout = 100000; // 100 Seconds

    public const string Key = "Web";

    public int PooledConnectionLifetime { get; set; } = DefaultPooledConnectionLifetime;

    public int Timeout { get; set; } = DefaultTimeout;
}