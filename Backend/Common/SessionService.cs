using Backend.Authenticate;
using Microsoft.AspNetCore.Session;
using System.Net;
using System.Text.Json;

namespace Backend.Common;

class SessionService(IHttpContextAccessor accessor)
{
    const string SESSION_KEY = "userinfo";

    public UserInfo? GetUserInfo()
    {
        var userInfoJson = accessor.HttpContext?.Session.GetString(SESSION_KEY) ?? "";

        if (string.IsNullOrWhiteSpace(userInfoJson) ||
            JsonSerializer.Deserialize<UserInfo>(userInfoJson) is not UserInfo userInfo)
            return null;

        return userInfo;
    }

    public void SetSession(UserInfo userInfo)
    {
        var json = JsonSerializer.Serialize(userInfo);
        accessor.HttpContext?.Session.SetString(SESSION_KEY, json);
    }
}
