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