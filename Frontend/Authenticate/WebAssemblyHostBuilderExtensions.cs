using Frontend.Authenticate.HttpHandlers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Frontend.Authenticate;

static class WebAssemblyHostBuilderExtensions
{
    public static void AddBFFAuthentication(this WebAssemblyHostBuilder builder)
    {
        var bi = builder.Configuration.GetSection("BackendInfo").Get<BackendInfo>() ?? throw new Exception("Configuration failed");

        builder.Services.AddScoped(sp => bi);
        builder.Services
            .AddSingleton<BackendUnauthorizedEventBroker>()
            .AddTransient<CookieAttachHandler>()
            .AddTransient<UnauthorizedResponseHandler>()
            .AddTransient<LogoutRequestHandler>()
            .AddSingleton<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Backend"))
            .AddHttpClient("Backend", hc => hc.BaseAddress = new Uri(bi.Host))
            .AddHttpMessageHandler<CookieAttachHandler>()
            .AddHttpMessageHandler<LogoutRequestHandler>()
            .AddHttpMessageHandler<UnauthorizedResponseHandler>();
        ;

        builder.Services.AddScoped<AuthenticationStateProvider, BackendAuthenticationStateProvider>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
    }
}