using System.Net;

namespace Frontend.Authenticate.HttpHandlers;

class UnauthorizedResponseHandler(BackendUnauthorizedEventBroker broker) : DelegatingHandler
{
    const string EMPTYJSON = "{ }";
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var message = base.Send(request, cancellationToken);
        Handle(message);
        return message;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var message = await base.SendAsync(request, cancellationToken);
        Handle(message);
        return message;
    }

    void Handle(HttpResponseMessage message)
    {
        // todo : may need more details
        if (message.StatusCode == HttpStatusCode.Unauthorized)
        {
            broker.Publish();      
            message.StatusCode = HttpStatusCode.OK;
            message.Content = new StringContent(EMPTYJSON);
        }
    }
}