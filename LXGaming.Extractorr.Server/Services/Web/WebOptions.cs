namespace LXGaming.Extractorr.Server.Services.Web;

public class WebOptions {

    public const int DefaultTimeout = 100000; // 100 Seconds

    public const string Key = "Web";

    public int Timeout { get; set; } = DefaultTimeout;
}