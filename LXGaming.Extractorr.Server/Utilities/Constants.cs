using System.Net.Http.Headers;
using System.Reflection;
using LXGaming.Common.Utilities;

namespace LXGaming.Extractorr.Server.Utilities;

public static class Constants {

    public static class Application {

        public const string Id = "extractorr";
        public const string Name = "Extractorr";
        public const string Authors = "Alex Thomson";
        public const string Source = "https://github.com/LXGaming/Extractorr";
        public const string Website = "https://lxgaming.me/";

        public static readonly string Version = AssemblyUtils.GetVersion(Assembly.GetExecutingAssembly(), "Unknown");
        public static readonly string UserAgent = $"{Name}/{Version} (+{Website})";
    }

    public static class AuthenticationSchemes {

        public const string Basic = "Basic";
    }

    public static class MediaTypeHeaderValues {

        public static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json");
    }
}