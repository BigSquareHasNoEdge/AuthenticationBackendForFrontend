using Backend.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Authenticate.Login;

class Get : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{provider}", Results<BadRequest, RedirectHttpResult> (
            string provider,
            [FromQuery] string returnUrl,
            [FromServices]OpenIdProvider[] providers,
            [FromServices]StateProtector protector) =>
        {
            var idp = providers.FirstOrDefault(x => provider.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));

            if (idp is null) return TypedResults.BadRequest();

            var state = new LoginRequestState(provider, returnUrl);
            var stateJson = JsonSerializer.Serialize(state);
            var protectedState = protector.Protect(stateJson);

            var authzEndpoint = idp.AuthzRequestEndpoint(protectedState);            
            return TypedResults.Redirect(authzEndpoint);
        });
    }
}
