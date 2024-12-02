using Frontend.Authenticate;
using Frontend.DataBags;
using Microsoft.AspNetCore.Components;

namespace Frontend.Pages;

[Route(PATH)]
public partial class Logins
{
    [Inject] BackendInfo BackendAInfo { get; set; } = null!;
    const string PATH = "/logins";
    public static string Path() => PATH;
    [CascadingParameter] internal ReturnUrlBag? ReturnUrlBag { get; set; }

}