using Backend.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Authenticate.Login;

class Get : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{provider}", (
            string provider,
            [FromQuery] string? returnUrl,
            AuthorizationCodeFlowHelper helper,
            StateProtector protector) =>
        {            
            // Todo client implementation
            var clientPageForServerError = "";

            var location = helper.GetGrantLocation(provider, clientPageForServerError);

            var state = new GrantRequestState(provider, returnUrl);
            var stateJson = JsonSerializer.Serialize(state);
            var protectedState = protector.Protect(stateJson);

            location += $"&state={protectedState}";
            
            return TypedResults.Redirect(location);
        });
    }
}
