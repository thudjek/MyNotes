using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Headers;
using WebApp.Auth;
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
        if (await ShouldRefreshToken())
        {
            if (await _refreshTokenService.TryRefreshToken())
            {
                var accessToken = await _localStorage.GetItemAsync<string>("accessToken");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else
            {
                await _localStorage.RemoveItemAsync("accessToken");
                _navigationManager.NavigateTo("login", true);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<bool> ShouldRefreshToken()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Claims.Any())
        {
            var exp = user.FindFirst(c => c.Type.Equals("exp")).Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));

            var timeUTC = DateTime.UtcNow;

            var diff = expTime - timeUTC;
            if (diff.TotalSeconds <= 90)
                return true;
        }

        return false;
    }
}