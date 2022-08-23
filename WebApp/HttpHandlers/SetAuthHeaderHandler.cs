using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace WebApp.HttpHandlers;

public class SetAuthHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    public SetAuthHeaderHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _localStorage.GetItemAsync<string>("accessToken");
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
