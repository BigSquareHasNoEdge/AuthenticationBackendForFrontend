# AuthenticationBackendForFrontend

This solution is purposed to implement BFF(Backend For Frontend) pattern, which is suggested by [IETF draft for OAuth 2.0 for Browswer based apps](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-browser-based-apps-19).


## System Layout  

The solution will end up with two application projects and one class library.

1. Backend  
Asp.Net Core Minimal Api app. 

1. Backed.Contract  
A class library containing data transfer models promised by Backend.

1. Frontend  
Blazor Webassembly Standalone app.

Each of Apps will be hosted at different servers(domains).   


## Backend App

### Configuration for OpenId Provider's Credentials


Backend project has a configuration file named "authenticates.json" for end points necessary for authorization code flow like below:

```json
{
  "OpenIdProviders": [
    {
      "Name": "Google",
      "AuthzEndpoint": "https://accounts.google.com/o/oauth2/auth?response_type=code&scope=openid email profile&redirect_uri={Your return url here}&client_id={Your Client ID Here}",
      "TokenEndpoint": "https://oauth2.googleapis.com/token?grant_type=authorization_code&redirect_uri={Your return url here}&client_id={Your Client Id Here}&client_secret={Your Client Secret Here}"
    }
  ]
}
```

As you can see, the end points of authorization server were hard-coded based on credentials received from Google OAuth Api.

You can make your own one by replacing placeholders, marked `{Your ... here}` in the above code block, with your ones.
Later, you can add more sections for other open id providers.

FYI, Google Api will give you a json file containing all relevant credentials, once you finished with OAUTH settings.
Downloading it into your local developing machine is strongly recommended for future reuse.

The configuration will be deserialized into this model:

```
namespace Backend.Common;

record OpenIdProvider(string Name, string AuthzEndpoint, string TokenEndpoint)
{
    public string GetTokenEndpoint(string authzCode) => TokenEndpoint + $"&code={authzCode}";

    public string AuthzRequestEndpoint(string? state) =>
        state is null ? AuthzEndpoint : AuthzEndpoint + $"&state={state}";
}
```

By the way, this repository ignores the file from my machine for my private security reason.  
As `BackendProgram.cs` will throw unless the file exists, 

```csharp
// ...

builder.Configuration.AddJsonFile("authenticates.json");

var providers = builder.Configuration.GetRequiredSection("OpenIdProviders").Get<OpenIdProvider[]>()
    ?? throw new InvalidOperationException("OpenIdProviders were not configured");
	
// ...
```

***you are required to add your own one before running the app.***

In the Backend project, addresses of sub systems are configured as "https://localhost:5004" at `./Properties/launchsettings.json` and Frontend one is as "https://localhost:7004" at the section of `ClientHost` in `./appsettings.json`.  

You are free to change them but be sure to make values sync with the contents of `authenticates.json`.


### Implementation details

#### Responses

Backend app's *CORS Middleware* reponses 404 against any request from other than the client host, which is `https://localhost:7004` in this solution.  

Otherwise,

- 400: when validation fails at each endpoint, especially, in `/callback/{provider}`.
- 302: only when aquisiting authorization code step.
- 200: for GET request whether or not it has the resouce.
- 401: *Authorization middleware* responses against unauthenticated, therefore unauthorized, access.    
The middleware relies on CookieAuthenticationScheme, which is configured, by default, to response 302 to backend's `/Account/Login`.  
This behavior was tweaked by forwarding its `Challenge` action to `BasicScheme`, which response 401.


#### Protections to malicious request

Once a user authenticated, the browser is redirected to front-end app's origin, where Blazor Webassembly app is downloaded and executed from the scratch.
Any request from other than the front-end origin is rejected.

There is no anti-forgery token to protect form submission at this stage for the simplicity.
When any vulnerability is found in the future, it will be added.


## Frontend app

### Implementation details

#### Protection at client side.

Frontend app has two weather pages, one(`WeatherOrigin`) is fetching the data from its Origin server, which is `https:localhost:7004` in the solution.  
It is the same with that of the .net project template.
This component is protected by `[Authorize]` at component level.

The other one(`WeatherBackend`) is doing from the Backend and not protected at component level nor at its children levels.

You can find some some comments at each page, for the effect of the protection.


#### Delegating handlers

Frontend app havily relies on `DelegatingHandler`s for the communication wth Backend.

#### Cascading Value

In order to conceal data shared between routable components, A.K.A. pages, I used Cascading system of blazor.
With that, the `Logins` page receive returnUrl value to send Backend from cascaded `ReturnUrlBag`.
This helps maintain simple route format without query parameters.