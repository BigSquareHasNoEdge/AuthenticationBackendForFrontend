using Backend.Common;

namespace Backend.Authenticate.Login;

static class Mappings
{
    public static void MapLogin(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/login")
            .AllowAnonymous()
            ;

        group.Has<Get>();
    }
}