using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
namespace WebApp.Services;

public class InterceptingHttpRequestHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;
    public InterceptingHttpRequestHandler(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = default(HttpResponseMessage);
        IncludeCookiesInRequest(request);
        await IncludeAccessTokenInAuthHeader(request);

        try
        {
            response = await base.SendAsync(request, cancellationToken);
        }
        catch(Exception ex)
        {
            var mess = ex.Message;
            throw;
        }
        finally
        {
            if (response != null)
            {
                //
            }
        }

        return response;
    }

    private static void IncludeCookiesInRequest(HttpRequestMessage request)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
    }

    private async Task IncludeAccessTokenInAuthHeader(HttpRequestMessage request)
    {
        var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");
        if (request.Headers.Authorization == null && !string.IsNullOrWhiteSpace(accessToken))
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
    }
}