using Backend.Common;
using Backend.Contract.Weather;

namespace Backend.Weather;

class Get : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        string[] summary =
        [
           "Freezing", "Bracing", "Chilly", "Cool",
            "Mild", "Warm", "Balmy",
            "Hot", "Sweltering", "Scorching"
        ];

        app.MapGet("", () =>
        {
            var forecast = Enumerable.Range(1, 5)
           .Select(index =>
           new WeatherForecast
           (
               DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
               Random.Shared.Next(-20, 55),
               summary[Random.Shared.Next(summary.Length)]

           ))
           .ToArray();
            return TypedResults.Ok(new GetResponse(forecast));
        });
    }
}
