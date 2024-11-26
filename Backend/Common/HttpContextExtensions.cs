using Backend.Authenticate;
using Microsoft.AspNetCore.Session;
using System.Text.Json;

namespace Backend.Common;

static class HttpContextExtensions
{
    const string SESSION_KEY = "userinfo";

    public static UserInfo? GetUserInfo(this HttpContext context)
    {
        var userInfoJson = context.Session.GetString(SESSION_KEY) ?? "";

        if (string.IsNullOrWhiteSpace(userInfoJson) ||
            JsonSerializer.Deserialize<UserInfo>(userInfoJson) is not UserInfo userInfo)
            return null;

        return userInfo;
    }

    public static void SetSession(this HttpContext context, UserInfo userInfo)
    {
        var json = JsonSerializer.Serialize(userInfo);
        context.Session.SetString(SESSION_KEY, json);
    }

    public static void RemoveSession(this HttpContext context)
    {
        context.Response.Cookies.Delete(SessionDefaults.CookieName);
        context.Session.Clear();
    }
}
