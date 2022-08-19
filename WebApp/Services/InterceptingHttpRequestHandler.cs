using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using SharedModels;
using System.Net;
using System.Net.Http.Json;
using WebApp.Extensions;

namespace WebApp.Services;

public class InterceptingHttpRequestHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;
    public InterceptingHttpRequestHandler(ILocalStorageService localStorageService, NavigationManager navigationManager)
    {
        _localStorageService = localStorageService;
        _navigationManager = navigationManager;
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
        catch
        {
            throw new ApplicationException(new ErrorModel().Error); //logging maybe?
        }
        finally
        {
            if (response != null && !response.IsSuccessStatusCode)
            {
                await HandleUnsuccessfulStatusCode(response);
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

    private async Task HandleUnsuccessfulStatusCode(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                await _localStorageService.RemoveItemAsync("accessToken");
                _navigationManager.NavigateTo("login", true);
                break;
            default:
                await response.ThrowExceptionFromErrorModel();
                break;
        }
    }
}