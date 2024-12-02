using Backend.Common;
using Backend.Contract.Authenticate.SessionCheck;
using System.Security.Claims;

namespace Backend.Authenticate.SessionCheck;

class Get : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", (ClaimsPrincipal user) =>
        {
            var si = SessionInfo.From(user);
            return TypedResults.Ok(si);
        });
    }
}