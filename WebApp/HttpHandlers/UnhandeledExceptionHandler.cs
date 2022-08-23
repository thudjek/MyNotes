using SharedModels;

namespace WebApp.HttpHandlers;

public class UnhandeledExceptionHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch
        {
            throw new ApplicationException(new ErrorModel().Error); //logging maybe?
        }
    }
}
