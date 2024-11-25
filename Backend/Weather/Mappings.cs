using Backend.Common;

namespace Backend.Weather;

static class Mappings 
{
    public static void MapWeathers(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/weatherforecast");

        group.Has<Get>();
        group.Has<GetByDay>();
    }
}
