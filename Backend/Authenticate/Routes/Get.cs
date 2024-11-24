using Backend.Common;
using Backend.Contract.Authenticate.AuthPath;

namespace Backend.Authenticate.Routes;

class Get : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", () =>
        {
            var rspns = new GetResponse("/auth/login", "/auth/logout", "/auth/sessioncheck");
            return TypedResults.Ok(rspns);
        });
    }
}