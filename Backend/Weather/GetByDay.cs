using Backend.Common;
using Backend.Contract.Weather;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend.Weather;

class GetByDay : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        string[] summary =
        [
           "Freezing", "Bracing", "Chilly", "Cool",
            "Mild", "Warm", "Balmy",
            "Hot", "Sweltering", "Scorching"
        ];

        app.MapGet("/{day}", Results<BadRequest, Ok<GetResponse>> (int day) =>
        {
            if (day is < 0 or > 5) return TypedResults.BadRequest();
            var forecast = new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(day)),
                Random.Shared.Next(-20, 55),
                summary[Random.Shared.Next(summary.Length)]
            );

            return TypedResults.Ok(new GetResponse([ forecast ]));
        });
    }
}
