using Frontend;
using Frontend.Authenticate;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("Origin", hc =>
{
    hc.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.AddBFFAuthentication();

await builder.Build().RunAsync();