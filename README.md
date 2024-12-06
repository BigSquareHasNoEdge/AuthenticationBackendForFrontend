# AuthenticationBackendForFrontend



This repository is purposed to compose a project(solution) template for  OAuth authentication in the BFF(Backend For Frontend) pattern, which is suggested by [IETF draft for OAuth 2.0 for Browswer based apps](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-browser-based-apps-19).

As you might know, there is no .Net template to implement this pattern.

For short introduction, BFF pattern for authentication helps minimize security risks when OAuth client is public.  
Normally, the public OAuth client stores its own credentials, directly receives user's credentials and handles security tokens from authroization servers by itself. Storing and handling them at client-side is vulnerable as a user easily can see them.  

BFF pattern requires all authorization works to be handled by backend api, who stores client credentials and store tokens. As a result, a client-side app just remains as a consumer of api service without knowing about them, which helps secure your system from attackers.

In addition, backend api doesn't receive user's credentials directly either, which helps secure user's credentials from your system.

## System Layout  


The solution consists of two application projects and a class library.

1. Backend  
Asp.Net Core Minimal Api app. 

1. Backed.Contract  
A class library distributing data transfer models promised by Backend.

1. Frontend  
Blazor Webassembly Standalone app.

Each of apps is assumed to be hosted at different host address.   
In real world, Frontend app is hosted by static web server like CDN and Backend app is cloud or self hosting environment.

## Backend App


### Configuration for OpenId Provider's Credentials

Backend project has a configuration file named "authenticates.json".  
It contains Uris of a authorization server with parameters configured to process Authorization Code Flow.

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

As you can see, the Uris were hard-coded based on credentials received from Google OAuth Api.

You can make your own one by replacing placeholders, marked `{Your ... here}` in the above code block, with your own credenticals. Later, you can add more sections for other open id providers.

FYI, Google Api will give you a json file containing all relevant credentials, once you finished with OAuth configuration in Google Api console. Downloading it into your local developing machine is strongly recommended for future reuse.

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

Backend address is configured as "https://localhost:5004" at `./Properties/launchsettings.json` and Frontend one is as "https://localhost:7004" at the section of `ClientHost` in `./appsettings.json`.  

You are free to change them but be sure to make values sync with the contents of `authenticates.json`.


### Implementation details

#### Responses

Backend app's *CORS Middleware* reponses 404 against any non-browser request from other than the client host.

Otherwise,

- 400: when validation fails at each endpoint, especially, in `/callback/{provider}`.
- 302: only when getting authorization code.
- 200: for valid GET requests whether or not Backend has the resouce for it.
- 401: *Authorization middleware* responses against unauthenticated, therefore unauthorized, access.    
This middleware relies on CookieAuthenticationScheme, which , by default, responses 302 to Backend's `/Account/Login`.  However, this behavior was tweaked by forwarding its `Challenge` action to `BasicScheme`, which responses 401.


#### Protections

Once a user is authenticated by Authorization server and, then, issued Authorization ticket stored in session Cookie, the browser is redirected to front-end app's origin that Backend knows.
So, any middle man can't catch the Backend's cookie reponse nor a replay attack is possible.

Currently, however, Backend doesn't have cross forgery protection because it actually doesn't have any endpoint expecting form data. When your need it, you can use anti-forgery support from Asp.net core by following the guide at:

https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-9.0#antiforgery-with-minimal-apis

Any how, if you find any vulnerability in the future, place an issue for it.


## Frontend app

This project is based on "Blazor Webassembly Standalone" template with no authentication configuration.

> If you chose "Indivitual Account" during creating the project, it would be configured for "Implicit OIDC Authorization flow". It is one of normal ways but doesn't conform to BFF pattern suggestion: "Nothing at client-side". 

As you can see, this app doesn't have functionalities 

- To store client's credentials  
- To received user credentials  
- To handle tokens from authorization server  


here is no javascript to store tokens 

>Note
> 

### Implementation details

#### Protections.

Frontend app has two weather pages, one(`WeatherOrigin`) is fetching the data from its Origin server, which is `https:localhost:7004` in the solution.  this implementation is the same with that of the project template of "Blazor Webassembly Standalone". The difference is WeatherOrigin is protected by `[Authorize]` at component level.

The other one(`WeatherBackend`) is doing from the Backend and not protected at component level nor at its children levels.

You can find some some comments at each page, for the effects of the protection variation.


#### Delegating handlers

Frontend app havily relies on `DelegatingHandler`s for the communication wth Backend.

#### Cascading Value

In order to conceal data shared between routable components, A.K.A. pages, I used Cascading system of blazor.
With that, the `Logins` page receive returnUrl value to send Backend from cascaded `ReturnUrlBag`.
This helps maintain simple route format without query parameters.