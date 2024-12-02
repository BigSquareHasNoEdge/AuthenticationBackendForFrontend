using Backend.Authenticate;
using Backend.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Callback;

public class Google : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/google", async Task<Results<BadRequest, RedirectHttpResult>> (
            HttpContext context,
            [FromQuery] string? state,
            [FromQuery] string code,
            [FromServices] StateProtector protector,
            [FromServices] IHttpClientFactory httpClientFactory,
            [FromServices] OAuthIdTokenHandler idTokenHandler,
            [FromKeyedServices("google")] OpenIdProvider provider) =>
        {
            if (state is not null
                && protector.Unprotect(state) is string json
                && JsonSerializer.Deserialize<LoginRequestState>(json) is LoginRequestState requestState
                && requestState.ReturnUrl is not null
                && requestState.Provider.Equals(provider.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var tokenEndpoint = provider.GetTokenEndpoint(code);

                var tokenEndpointResponse = await httpClientFactory.CreateClient().PostAsync(tokenEndpoint, null);
                var idTokenResponse = await tokenEndpointResponse.Content.ReadFromJsonAsync<TokenEndpointResponse>();

                var principal = idTokenHandler.GetPrincipal(idTokenResponse?.Id_Token, provider.Name);

                if (principal.Identity?.IsAuthenticated == true)
                {
                    await context.SignInAsync(principal);
                    return TypedResults.Redirect(requestState.ReturnUrl);
                }
            }
            return TypedResults.BadRequest();
        }).DisableAntiforgery();
        ;
    }
}
