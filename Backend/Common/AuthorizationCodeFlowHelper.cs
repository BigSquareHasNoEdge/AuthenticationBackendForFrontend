
namespace Backend.Common;

class AuthorizationCodeFlowHelper(OpenIdProvider[] idps)
{
    const string SCOPE = "openid email profile";
    const string RESPONSE_TYPE = "code";
    public bool IsSupporting(string providerName) => 
        idps.Any(x => x.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));
    
    public string GetGrantLocation(string providerName, string whenNotSupporting) =>
        idps.FirstOrDefault(x => x.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase))
        ?.GrantLocation(RESPONSE_TYPE, SCOPE)
        ?? whenNotSupporting;

    internal IEnumerable<OpenIdProvider> Providers() => idps;
    
}