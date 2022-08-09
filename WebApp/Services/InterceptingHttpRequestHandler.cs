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
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");
        if (request.Headers.Authorization == null && !string.IsNullOrWhiteSpace(accessToken))
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

        return await base.SendAsync(request, cancellationToken);
    }
}
