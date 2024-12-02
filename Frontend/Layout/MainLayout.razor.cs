using Frontend.DataBags;
using Frontend.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Frontend.Layout;

partial class MainLayout : IDisposable
{
    [Inject]
    public NavigationManager NavMan { get; set; } = null!;

    [CascadingParameter]
    internal ReturnUrlBag ReturnUrlBag { get; set; } = null!;

    IDisposable? _navigatingHandler;

    protected override void OnInitialized()
    {        
        _navigatingHandler = NavMan.RegisterLocationChangingHandler(OnNavigating);
    }

    private ValueTask OnNavigating(LocationChangingContext context)
    {
        if(context.TargetLocation.Contains(Logins.Path()))
            ReturnUrlBag.Set(NavMan.Uri);

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _navigatingHandler?.Dispose();
    }
}