using Backend.Common;

namespace Backend.Authenticate.SessionCheck;

static class Mappings
{
    public static void MapSessionChecks(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/sessioncheck");

        group.Has<Get>();
    }
}