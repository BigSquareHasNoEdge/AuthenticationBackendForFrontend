using Backend.Authenticate;
using Backend.Common;
using Backend.GrantCallbacks;
using Backend.Weather;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("authenticates.json");
var providers = builder.Configuration.GetRequiredSection("OpenIdProviders").Get<OpenIdProvider[]>()
    ?? throw new InvalidOperationException("OpenIdProviders were not configured");
builder.Services.AddSingleton<AuthorizationCodeFlowHelper>(sp => new (providers));


builder.Services.AddHttpClient();

builder.Services.AddDataProtection();
builder.Services
    .AddTransient<StateProtector>();

var clientHost = builder.Configuration.GetRequiredSection("ClientHost").Value
    ?? throw new InvalidOperationException("ClientHost was not configured");

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
            policy.WithOrigins(clientHost)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()));

builder.Services
    .AddHttpContextAccessor()
    .AddTransient<SessionService>();

builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, SessionCookieSchemeHandler>("SessionCookie", null);

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
api.MapAuths();

var helper = app.Services.GetRequiredService<AuthorizationCodeFlowHelper>();
api.MapGrantCallbacks(helper.Providers());

app.Run();