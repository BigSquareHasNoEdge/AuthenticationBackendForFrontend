using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Backend.Common;

class OAuthIdTokenHandler(OpenIdProvider[] supportedProviders)
{
    readonly JsonWebTokenHandler _handler = new ();
    public bool IsWellFormed(string? idTokenString) => !string.IsNullOrWhiteSpace(idTokenString) 
        && _handler.CanReadToken(idTokenString);

    public ClaimsPrincipal GetPrincipal(string? idTokenString, string providerName)
    {
        if (IsWellFormed(idTokenString) is false ||
            !supportedProviders.Any(p => providerName.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase))) 
            return new();

        var jwt = new JsonWebToken(idTokenString);

        return providerName.ToLower() switch
        {
            "google" => ParseGoogleIdToken(jwt),
            _ => new()
        };
    }

    private static ClaimsPrincipal ParseGoogleIdToken(JsonWebToken jwt)
    {
        const string AuthType = "Google OAuth";
        if (jwt.TryGetClaim("email", out var emailClaim))
        {
            if(jwt.TryGetClaim("name", out var nameClaim))
            {
                var email = emailClaim.Value;
                var name = nameClaim.Value;
                return new(RigitIdentity((email, name, "google"), AuthType));
            }
        }

        return new();
    }

    private static ClaimsIdentity RigitIdentity((string Email, string Name, string provider) t, string authType) => 
        new ( [ new (ClaimTypes.Email, t.Email), new(ClaimTypes.Name, t.Name), new("Provider", t.provider) ],  authType);
}