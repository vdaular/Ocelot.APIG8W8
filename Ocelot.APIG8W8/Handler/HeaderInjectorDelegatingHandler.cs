namespace Ocelot.APIG8W8.Handler;

public class HeadersInjectorDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var host = $"{httpContextAccessor.HttpContext!.Request.Host.Value}";

        request.Headers.Add("Host", host);

        return base.SendAsync(request, cancellationToken);
    }
}
