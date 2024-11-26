using Backend.Common;

namespace Backend.Callback;

static class Mappings
{
    public static void MapCallbacks(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/callback")
            .AllowAnonymous()
            ;
        group.Has<Google>();
    }
}
