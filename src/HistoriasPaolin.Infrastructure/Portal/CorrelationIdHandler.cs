using Microsoft.AspNetCore.Http;

namespace HistoriasPaolin.Infrastructure.Portal;

public sealed class CorrelationIdHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    public const string HeaderName = "X-Correlation-ID";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
        request.Headers.TryAddWithoutValidation(HeaderName, correlationId);
        return base.SendAsync(request, cancellationToken);
    }
}
