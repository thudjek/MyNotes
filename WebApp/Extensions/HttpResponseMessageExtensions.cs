using SharedModels;
using System.Net.Http.Json;
using WebApp.Common.Exceptions;

namespace WebApp.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task ThrowExceptionFromErrorModel(this HttpResponseMessage response)
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
