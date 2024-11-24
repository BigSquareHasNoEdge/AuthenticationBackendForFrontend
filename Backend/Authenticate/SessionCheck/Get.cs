using Backend.Common;
using System.Security.Claims;

namespace Backend.Authenticate.SessionCheck;

class Get : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", (ClaimsPrincipal user) =>
        {
            var rspns = user.ToSessionCheckResponse();
            return TypedResults.Ok(rspns);
        });
    }
}