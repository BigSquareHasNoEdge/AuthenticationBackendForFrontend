namespace Frontend.Authenticate.HttpHandlers;

class BackendUnauthorizedEventBroker
{
    public event Action? Unauthorized;
    public void Publish() => Unauthorized?.Invoke();
}