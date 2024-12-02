using Backend.Contract.Authenticate.SessionCheck;
using Frontend.Authenticate.HttpHandlers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Frontend.Authenticate;

class BackendAuthenticationStateProvider : AuthenticationStateProvider
{
    public BackendAuthenticationStateProvider(HttpClient client, BackendInfo backend, BackendUnauthorizedEventBroker broker)
    {
        _backendClient = client;
        _backendInfo = backend;
        _broker = broker;
        _broker.Unauthorized -= On401Responsed;
        _broker.Unauthorized += On401Responsed;
    }
    readonly AuthenticationState _anonymous = new (new());
    private readonly HttpClient _backendClient;
    private readonly BackendInfo _backendInfo;
    readonly BackendUnauthorizedEventBroker _broker;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var response = await _backendClient.GetAsync(_backendInfo.SessionCheck);
            if (response.IsSuccessStatusCode is false) return _anonymous;

            var si = await response.Content.ReadFromJsonAsync<SessionInfo>();
            if (si is null) return _anonymous;

            if (_backendInfo.Providers.Any(x => x.Equals(si.Provider, StringComparison.InvariantCultureIgnoreCase)) is false)
                return _anonymous;

            var ci = new ClaimsIdentity([new Claim(ClaimTypes.Email, si.Email), new(ClaimTypes.Name, si.Name)], si.Provider);
            return new(new(ci));
        }
        catch (Exception)
        {
            return _anonymous;
        }
    }

    void On401Responsed()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));        
    }
}