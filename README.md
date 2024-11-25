# AuthenticationBackendForFrontend

This solution is purposed to implement BFF(Backend For Frontend) pattern in terms of authentication based on OAuth 2.0, which is suggested by [IETF draft for OAuth 2.0 for Browswer based apps](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-browser-based-apps-19).

## System Layout  

The solution will end up with two application projects:

1. Backend  
Asp.Net Core Minimal Api app. 
3. Frontend  
Blazor Webassembly Standalone app.

Each of them will be hosted at different servers and Frontend app especially is assumed to be hosted at CDN.  


## Backend App

Backed project has a configuration for key values for authorization code flow.

```json
{
  "OpenIdProviders": [
    {
      "Name": "Google",
      "RedirectRoute": "/grant-callback/google",
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

record OpenIdProvider(
    string Name,
    string AuthzEndpoint,
    string RedirectRoute,
    string TokenEndpoint)
{
    public string GetTokenEndpoint(string authzCode) => TokenEndpoint + $"&code={authzCode}";

    public string AuthzRequestEndpoint(string? state) =>
        state is null ? AuthzEndpoint : AuthzEndpoint + $"&state={state}";

}
```

By the way, this repository doesn't include a file containg the above section for my private security reason.

As you can see, BackendProgram will throws unless the configuration was set up properly.

```csharp
// ...

builder.Configuration.AddJsonFile("authenticates.json");
var providers = builder.Configuration.GetRequiredSection("OpenIdProviders").Get<OpenIdProvider[]>()
    ?? throw new InvalidOperationException("OpenIdProviders were not configured");
	
// ...
```
You will need to create your own `.json` file to be populated with your backend's credentials.

The Backend address of "https://localhost:5004" is configured by `./Properties/launchsettings.json`, while Frontend is by the section of `ClientHost` in `./appsettings.json`.  

```
{
  ...
  "ClientHost" : "https://localhost:7004"
}
```

You are free to change them but be sure to make values sync with the OpenIdProviders section.



## Frontend app


[TBD]

