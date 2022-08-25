using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Headers;
using WebApp.Services;

namespace WebApp.HttpHandlers;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;
    private readonly RefreshTokenService _refreshTokenService;
    public RefreshTokenHandler(ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider, NavigationManager navigationManager, RefreshTokenService refreshTokenService)
    {
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
        _refreshTokenService = refreshTokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            if (await _refreshTokenService.TryRefreshToken())
            {
                var accessToken = await _localStorage.GetItemAsync<string>("accessToken");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = await base.SendAsync(request, cancellationToken);
            }
            else 
            {
                await _localStorage.RemoveItemAsync("accessToken");
                _navigationManager.NavigateTo("login", true);
            }       
        }

        return response;
    }
}