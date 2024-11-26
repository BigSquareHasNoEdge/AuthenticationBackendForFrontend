using Backend.Authenticate;
using Backend.Common;
using Backend.GrantCallbacks;
using Backend.Weather;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);


#region ConfigurationCheck
builder.Configuration.AddJsonFile("authenticates.json");
var providers = builder.Configuration.GetRequiredSection("OpenIdProviders").Get<OpenIdProvider[]>()
    ?? throw new InvalidOperationException("OpenIdProviders were not configured");

var clientHost = builder.Configuration.GetRequiredSection("ClientHost").Value
    ?? throw new InvalidOperationException("ClientHost was not configured");
#endregion

// register OpenIdProvider[]
builder.Services.AddSingleton(providers);

builder.Services.AddHttpClient();

# region Register services for session cookie authendtication
builder.Services.AddTransient<StateProtector>()
    .AddDataProtection();

builder.Services.AddDistributedMemoryCache()
    .AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, SessionCookieSchemeHandler>("SessionCookie", null);

builder.Services
    .AddHttpContextAccessor();

#endregion

builder.Services.AddScoped<OAuthIdTokenHandler>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(clientHost)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod()));

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

api.MapGrantCallbacks(providers);

app.Run();