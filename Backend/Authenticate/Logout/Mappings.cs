using Backend.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Session;

namespace Backend.Authenticate.Logout;

static class Mappings
{
    public static void MapLogout(this IEndpointRouteBuilder app)
    {
        app.MapGet("/logout", (HttpContext http) =>
        {            
            http.Response.Cookies.Delete(SessionDefaults.CookieName);
            http.Session.Clear();
            return TypedResults.NoContent();
        });
    }
}