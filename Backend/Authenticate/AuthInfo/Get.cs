using Backend.Common;
using Backend.Contract.Authenticate.Routes;

namespace Backend.Authenticate.Routes;

class Get : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", (OpenIdProvider[] providers) =>
        {
            var names = providers.Select(p => p.Name).ToArray();

            var rspns = new GetResponse("/auth/login", "/auth/logout", "/auth/sessioncheck", names);
            return TypedResults.Ok(rspns);
        });
    }
}