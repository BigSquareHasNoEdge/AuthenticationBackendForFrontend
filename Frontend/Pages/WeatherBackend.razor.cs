using Backend.Contract.Weather;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Frontend.Pages;

[Route(PATH)]
public partial class WeatherBackend
{
    public const string PATH = "/weather/backend";
    private WeatherForecast[]? forecasts;

    [Inject]
    HttpClient Http { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        forecasts = (await Http.GetFromJsonAsync<GetResponse>("/weatherforecast"))?.WeatherForecasts
         ?? [];
    }
}