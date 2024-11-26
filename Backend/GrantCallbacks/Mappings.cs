using Backend.Authenticate;
using Backend.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.GrantCallbacks;

static class Mappings
{
    public static void MapGrantCallbacks(this IEndpointRouteBuilder app, IEnumerable<OpenIdProvider> providers)
    {
        var group = app.MapGroup("")
            .AllowAnonymous();

        foreach (var provider in providers)
        {
            group.MapGet(provider.RedirectRoute, async (
                [FromQuery] string? state,
                [FromQuery(Name = "code")] string authzCode,
                StateProtector protector,
                HttpContext context,
                IConfiguration config,
                IHttpClientFactory httpClientFactory,
                OAuthIdTokenHandler idTokenHandler) =>
            {
                var redirectLocation = config.GetRequiredSection("ClientHost").Value;

                if (state != null && protector.Unprotect(state) is string json &&
                   JsonSerializer.Deserialize<GrantRequestState>(json) is GrantRequestState requestState &&
                   requestState.Provider.Equals(provider.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    var tokenEndpoint = provider.GetTokenEndpoint(authzCode);
                    var tokenEndpointResponse = await httpClientFactory.CreateClient().PostAsync(tokenEndpoint, null);
                    var idTokenResponse = await tokenEndpointResponse.Content.ReadFromJsonAsync<TokenEndpointResponse>();

                    var idToken = idTokenResponse?.Id_Token ?? "";
                    var userInfo = idTokenHandler.GetUserInfo(idToken, provider.Name);

                    if (userInfo.IsValid())
                    {
                        context.SetSession(userInfo);
                        if (requestState.ReturnUrl is not null)
                            redirectLocation = requestState.ReturnUrl;
                    }
                }

                return TypedResults.Redirect(redirectLocation!);
            });
        }
    }
}
