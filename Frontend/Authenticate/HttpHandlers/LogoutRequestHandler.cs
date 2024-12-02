

namespace Frontend.Authenticate.HttpHandlers;

class LogoutRequestHandler(BackendInfo backendInfo, BackendUnauthorizedEventBroker broker) : DelegatingHandler
{
    private readonly BackendUnauthorizedEventBroker _broker = broker;
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {       
        Handle(request);
        return base.Send(request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Handle(request);
        return base.SendAsync(request, cancellationToken);
    }

    void Handle(HttpRequestMessage message)
    {
        if (message.RequestUri?.AbsoluteUri == backendInfo.Host + backendInfo.Logout)
        {
            _broker.Publish();
        }
    }
}