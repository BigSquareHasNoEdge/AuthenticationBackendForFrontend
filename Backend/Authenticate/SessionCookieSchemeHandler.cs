using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Backend.Authenticate;

internal class SessionCookieSchemeHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    SessionService userInfoSessionService)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        await Task.CompletedTask;

        var userInfo = userInfoSessionService.GetUserInfo();

        if (userInfo is null)
            return AuthenticateResult.Fail("Invalid request");

        var principal = userInfo.ToPrincipal(Scheme.Name);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}