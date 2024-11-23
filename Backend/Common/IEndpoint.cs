namespace Backend.Common;


interface IEndpoint
{
    /// <summary>
    /// Filters and meta data can be configured here.
    /// </summary>
    /// <param name="app"></param>
    abstract static void Map(IEndpointRouteBuilder app);
}
