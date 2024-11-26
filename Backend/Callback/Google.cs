using Backend.Authenticate;
using Backend.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Callback;

public class Google : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/google", async (
                [FromQuery] string? state,
                [FromQuery(Name = "code")] string authzCode,
                StateProtector protector,
                HttpContext context,
                ClientHost clientHost,
                IHttpClientFactory httpClientFactory,
                OAuthIdTokenHandler idTokenHandler,
                [FromKeyedServices("google")] OpenIdProvider provider) =>
        {            

            if (state != null && protector.Unprotect(state) is string json &&
               JsonSerializer.Deserialize<GrantRequestState>(json) is GrantRequestState requestState &&
               requestState.Provider.Equals(provider.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var redirectLocation = requestState.ReturnUrl ?? clientHost.Host;
                var tokenEndpoint = provider.GetTokenEndpoint(authzCode);
                var tokenEndpointResponse = await httpClientFactory.CreateClient().PostAsync(tokenEndpoint, null);
                var idTokenResponse = await tokenEndpointResponse.Content.ReadFromJsonAsync<TokenEndpointResponse>();

                var idToken = idTokenResponse?.Id_Token ?? "";

                var principal = idTokenHandler.GetPrincipal(idToken, provider.Name);

                if (principal.Identity?.IsAuthenticated == true)
                {
                    await context.SignInAsync(principal);
                    return TypedResults.Redirect(redirectLocation);
                }
            }

            return TypedResults.Redirect(clientHost.Host);
        })
        ;
    }
}
