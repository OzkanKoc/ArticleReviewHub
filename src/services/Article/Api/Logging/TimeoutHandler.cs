namespace Api.Logging;

/// <summary>
///     This class adds a custom timeout cancellation token to Http requests.
/// </summary>
public class TimeoutHandler : DelegatingHandler
{
    private int _timeout;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var cts = GetCancellationTokenSource(request, cancellationToken);

        try
        {
            return await base.SendAsync(request, cts?.Token ?? cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException
                ($"The request was canceled due to the configured 'HttpRequestOptions Timeout Property' of {_timeout} seconds elapsing.");
        }
    }

    private CancellationTokenSource GetCancellationTokenSource(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Options.TryGetValue(new HttpRequestOptionsKey<int>("timeout"), out var timeoutValue)) 
            return null;

        _timeout = timeoutValue;

        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(timeoutValue));

        return cts;
    }
}
