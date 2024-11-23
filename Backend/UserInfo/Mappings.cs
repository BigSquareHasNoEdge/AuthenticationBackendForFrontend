using Backend.Common;

namespace Backend.UserInfo;

static class Mappings 
{
    public static void MapUserInfos(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/userinfos")
            ;

        group.Has<Get>();
    }
}