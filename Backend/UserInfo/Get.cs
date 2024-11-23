using Backend.Common;
using Backend.Contract.UserInfo;
using System.Security.Claims;

namespace Backend.UserInfo;

class Get : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("", (ClaimsPrincipal user) =>
        {
            var rspns = new GetResponse(
            user.FindFirstValue(ClaimTypes.Email) ?? "",
            user.FindFirstValue(ClaimTypes.Name) ?? "");

            return TypedResults.Ok(rspns);
        });
    }
}