using Microsoft.AspNetCore.Authentication;

namespace Backend.Authenticate.Logout;

static class Mappings
{
    public static void MapLogout(this IEndpointRouteBuilder app)
    {
        app.MapGet("/logout", async (HttpContext http) =>
        {
            await http.SignOutAsync();
            return TypedResults.Ok();
        });
    }
}