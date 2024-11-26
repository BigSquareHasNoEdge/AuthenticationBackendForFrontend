using Backend;
using Backend.Authenticate;
using Backend.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


#region ConfigurationCheck
builder.Configuration.AddJsonFile("authenticates.json");

var providers = builder.Configuration.GetRequiredSection("OpenIdProviders").Get<OpenIdProvider[]>()
    ?? throw new InvalidOperationException("OpenIdProviders were not configured");

foreach (var provider in providers)
{
    var key = provider.Name.ToLower();

    builder.Services.AddKeyedSingleton(key, provider);
}

builder.Services.AddSingleton(sp => providers);
var clientHost = builder.Configuration.GetRequiredSection("ClientHost").Get<ClientHost>()
    ?? throw new InvalidOperationException("ClientHost was not configured");

builder.Services.AddSingleton(clientHost);

#endregion


builder.Services.AddHttpClient();

builder.Services.AddTransient<StateProtector>()
    .AddDataProtection();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        o.ForwardChallenge = "BasicScheme";
        o.Validate();
    })
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicScheme", o =>
    {
        o.ForwardAuthenticate = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    ;

builder.Services.AddScoped<OAuthIdTokenHandler>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(clientHost.Host)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.AddAppEndpoints();

app.Run();