using SharedModels;
using System.Net.Http.Json;
using WebApp.Common.Exceptions;
using WebApp.Extensions;

namespace WebApp.HttpHandlers;

public class UnsuccessfulStatusCodeHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            await ThrowExceptionFromFailedResponse(response);

        return response;   
    }

    private static async Task ThrowExceptionFromFailedResponse(HttpResponseMessage response)
    {
        var errorModel = new ErrorModel();

        try
        {
            errorModel = await response.Content.ReadFromJsonAsync<ErrorModel>();
        }
        catch
        {
            throw new ApplicationException(errorModel.Error);
        }

        if (errorModel.Errors != null)
            throw new ValidationException(errorModel);
        else
            throw new ApplicationException(errorModel.Error);
    }
}
