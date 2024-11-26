using Backend.Authenticate;
using Backend.Callback;
using Backend.Weather;

namespace Backend;

static class Endpoints
{
    public static void AddAppEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/").RequireAuthorization();
        
        api.MapWeathers();
        api.MapAuths();
        api.MapCallbacks();
    }
}