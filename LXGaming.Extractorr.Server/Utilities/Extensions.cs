using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace LXGaming.Extractorr.Server.Utilities;

public static class Extensions {

    public static IActionResult Unauthorized(this HttpContext context, string authenticationScheme) {
        context.Response.Headers["WWW-Authenticate"] = authenticationScheme;
        return new UnauthorizedResult();
    }

    public static bool VerifyBasicAuthentication(this HttpRequest request, string username, string password) {
        string? authorization = request.Headers.Authorization;
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith(Constants.AuthenticationSchemes.Basic, StringComparison.OrdinalIgnoreCase)) {
            return false;
        }

        var encodedValue = authorization[Constants.AuthenticationSchemes.Basic.Length..].Trim();
        if (string.IsNullOrEmpty(encodedValue)) {
            return false;
        }

        string value;
        try {
            value = Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));
        } catch (FormatException) {
            return false;
        }

        var arguments = value.Split(":", 2);
        if (arguments.Length != 2) {
            return false;
        }

        return StringUtils.FixedTimeEquals(username, arguments[0], Encoding.UTF8)
               && StringUtils.FixedTimeEquals(password, arguments[1], Encoding.UTF8);
    }
}