﻿namespace Backend.Contract.Weather;
public record GetResponse(WeatherForecast[] WeatherForecasts);
public record GetByDayResponse(WeatherForecast? WeatherForecasts);