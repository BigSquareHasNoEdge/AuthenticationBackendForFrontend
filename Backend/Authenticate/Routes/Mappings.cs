using Backend.Common;

namespace Backend.Authenticate.Routes;

static class Mappings
{
    public static void MapRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/routes")
            .AllowAnonymous()
            ;

        group.Has<Get>();
    }
}