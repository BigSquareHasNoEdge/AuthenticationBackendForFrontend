namespace Backend.Common;

record OpenIdProvider(string Name, string ClientId, string ClientSecret, string AuthUri, string TokenUri, string RedirectUri)
{
    public string GrantLocation(string responseType, string scope) =>
        $"{AuthUri}?response_type={responseType}&scope={scope}&redirect_uri={RedirectUri}&client_id={ClientId}";
    public string RedirectRoute() => new Uri(RedirectUri).PathAndQuery;

    public string TokenEndPoint(string authzCode) => $"{TokenUri}?&grant_type=authorization_code&code={authzCode}" +
        $"&redirect_uri={RedirectUri}&client_id={ClientId}&client_secret={ClientSecret}";
}