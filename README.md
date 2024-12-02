# AuthenticationBackendForFrontend

This solution is purposed to implement BFF(Backend For Frontend) pattern, which is suggested by [IETF draft for OAuth 2.0 for Browswer based apps](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-browser-based-apps-19).


## System Layout  

The solution will end up with two application projects:

1. Backend  
Asp.Net Core Minimal Api app. 

1. Backed.Contract  
A class library containing data models promised by Backend.

3. Frontend  
Blazor Webassembly Standalone app.

Each of Apps will be hosted at different servers(domains).   


## Backend App

Backed project has a configuration file named "Authenticates.json" for key values for authorization code flow like below:

```json
{
  "OpenIdProviders": [
    {
      "Name": "Google",
      "AuthzEndpoint": "https://accounts.google.com/o/oauth2/auth?response_type=code&scope=openid email profile&redirect_uri=https://localhost:5004/grant-callback/google&client_id={Your Client ID Here}",
      "TokenEndpoint": "https://oauth2.googleapis.com/token?grant_type=authorization_code&redirect_uri=https://localhost:5004/grant-callback/google&client_id={Your Client Id Here}&client_secret={Your Client Secret Here}"
    }
  ]
}
```

As you can see, the end points were hard-coded based on credentials received from Google OAuth Api.
Later, you can add more sections for other open id providers.

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

By the way, this repository doesn't include the file for my private security reason.  
As `BackendProgram.cs` will throws unless the file exists, you are required to add your own one before running the app.

```csharp
// ...

builder.Configuration.AddJsonFile("authenticates.json");

var providers = builder.Configuration.GetRequiredSection("OpenIdProviders").Get<OpenIdProvider[]>()
    ?? throw new InvalidOperationException("OpenIdProviders were not configured");
	
// ...
```


The Backend address of "https://localhost:5004" is configured by `./Properties/launchsettings.json`, while Frontend one is by the section of `ClientHost` in `./appsettings.json`.  

```
{
  ...
  "ClientHost" : "https://localhost:7004"
}
```

You are free to change them but be sure to make values sync with the OpenIdProviders section.


## Frontend app

Frontend app has two weather pages, one is fetching the data from its host(Origin) server.
It is the same with that of the project template.

However, the other is doing from the back-end.

I have set up the origin weather to protected by `<AuthorizeView>` but not the backend weather.
Please refer to some comments on each pages for why I did that.

Any comments or advices are welcome.