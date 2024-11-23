namespace Backend.Common;

static class MapGroupExtensions
{
    public static IEndpointRouteBuilder Has<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
