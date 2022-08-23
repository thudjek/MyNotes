using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SharedModels.Requests.Auth;
using SharedModels.Responses.Auth;
using System.Net.Http.Json;
using WebApp.Auth;

namespace WebApp.Services;

public class RefreshTokenService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    public RefreshTokenService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
    }

    public async Task<bool> TryRefreshToken()
    {
        try
        {
            var accessToken = await _localStorage.GetItemAsync<string>("accessToken");
            var refreshTokenRequest = new RefreshTokenRequest() { AccessToken = accessToken };
            var httpResponse = await _httpClient.PostAsJsonAsync("auth/refresh-token", refreshTokenRequest);

            if (!httpResponse.IsSuccessStatusCode)
                return false;

            var tokenResponse = await httpResponse.Content.ReadFromJsonAsync<TokenResponse>();
            await _localStorage.SetItemAsync("accessToken", tokenResponse.AccessToken);
            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(tokenResponse.AccessToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
