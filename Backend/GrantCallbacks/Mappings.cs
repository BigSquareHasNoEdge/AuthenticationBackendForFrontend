using Backend.Authenticate;
using Backend.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Text.Json;

namespace Backend.GrantCallbacks;

static class Mappings
{
    public static void MapGrantCallbacks(this IEndpointRouteBuilder app, IEnumerable<OpenIdProvider> providers)
    {
        foreach (var provider in providers)
        {
            app.MapGet(provider.RedirectRoute(), async (
                [FromQuery] string? state,
                [FromQuery(Name = "code")] string authzCode,
                StateProtector protector,
                SessionService session,
                IConfiguration config,
                IHttpClientFactory httpClientFactory) =>
            {
                var location = config.GetRequiredSection("ClientHost").Value;

                if (state != null && protector.Unprotect(state) is string json &&
                   JsonSerializer.Deserialize<GrantRequestState>(json) is GrantRequestState requestState &&
                   requestState.Provider.Equals(provider.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    var tokenEndpoint = provider.TokenEndPoint(authzCode);

                    var tokenEndpointResponse = await httpClientFactory.CreateClient().PostAsync(tokenEndpoint, null);
                    var idTokenResponse = await tokenEndpointResponse.Content.ReadFromJsonAsync<TokenEndpointResponse>();

                    #region Set Session
                    var idToken = idTokenResponse?.Id_Token;

                    var jwt = new JsonWebToken(idToken);

                    // Extract UserInfo from claims
                    var userInfo = new UserInfo
                    (
                        jwt.TryGetClaim("email", out var claim) ? claim.Value : "",
                        jwt.TryGetClaim("name", out claim) ? claim.Value : ""
                    );

                    if (userInfo.IsValid())
                    {
                        session.SetSession(userInfo);
                        if (requestState.ReturnUrl is not null)
                            location = requestState.ReturnUrl;
                    }
                    #endregion
                }

                return TypedResults.Redirect(location!);
            })
            .AllowAnonymous()
           ;
        }
    }
}
