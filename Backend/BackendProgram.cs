using Backend.Authenticate;
using Backend.UserInfo;
using Backend.Weather;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddDistributedMemoryCache()
    .AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddCors(options =>
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins("https://localhost:7004")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()))
    ;

builder.Services
    .AddHttpContextAccessor()
    .AddTransient<SessionService>()
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, SessionCookieSchemeHandler>("Session", null);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("")
    .RequireAuthorization();

api.MapWeathers();
api.MapUserInfos();

app.Run();