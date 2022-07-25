using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SharedModels.Requests.Auth;
using SharedModels.Responses.Auth;
using System.Net.Http.Json;
using WebApp.Auth;
using WebApp.Common;

namespace WebApp.Services;

public class RefreshTokenService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    public RefreshTokenService(AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage, IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
    {
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClientName);
        _navigationManager = navigationManager;
    }

    public async Task TryRefreshToken()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity.IsAuthenticated)
        {
            var exp = user.FindFirst(c => c.Type.Equals("exp")).Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 1)
            {
                await RefreshToken();
            }
        }
    }

    private async Task RefreshToken()
    {
        var accessToken = await _localStorage.GetItemAsync<string>("accessToken");
        var refreshTokenRequest = new RefreshTokenRequest() { AccessToken = accessToken };
        var httpResponse = await _httpClient.PostAsJsonAsync("auth/refresh-token", refreshTokenRequest);
        if (httpResponse.IsSuccessStatusCode)
        {
            var tokenResponse = await httpResponse.Content.ReadFromJsonAsync<TokenResponse>();
            await _localStorage.SetItemAsync("accessToken", tokenResponse.AccessToken);
            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(tokenResponse.AccessToken);
        }
        else
            _navigationManager.NavigateTo("logout");
    }
}
