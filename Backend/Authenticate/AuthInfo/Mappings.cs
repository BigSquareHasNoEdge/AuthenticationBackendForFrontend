using Backend.Authenticate.AuthInfo;
using Backend.Common;

namespace Backend.Authenticate.Routes;

static class Mappings
{
    public static void MapRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/authinfo")
            .AllowAnonymous()
            ;

        group.Has<Get>();
    }
}