using System.Text.Json;

namespace Backend.Authenticate;

class SessionService(IHttpContextAccessor accessor)
{
    public const string SESSION_KEY = "userinfo";

    public UserInfo? GetUserInfo()
    {
        var userInfoJson = accessor.HttpContext?.Session.GetString(SESSION_KEY) ?? "";

        if (string.IsNullOrWhiteSpace(userInfoJson) ||
            JsonSerializer.Deserialize<UserInfo>(userInfoJson) is not UserInfo userInfo)
            return null;

        return userInfo;
    }
}
