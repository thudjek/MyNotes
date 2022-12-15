using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Auth;
using WebApp.HttpHandlers;
using WebApp.Services;

namespace WebApp;

public static class ConfigureClientServices
{
    public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        return builder;
    }

    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
        builder.Services.AddScoped<PopupMessageService>();
        builder.Services.AddTransient<IncludeCookiesHandler>();
        builder.Services.AddTransient<SetAuthHeaderHandler>();
        builder.Services.AddTransient<UnsuccessfulStatusCodeHandler>();
        builder.Services.AddTransient<RefreshTokenHandler>();
        builder.Services.AddTransient<UnhandeledExceptionHandler>();
        builder.Services.AddTransient<RefreshTokenService>();

        builder.Services.AddHttpClient<ApiService>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
        })
        .AddHttpMessageHandler<IncludeCookiesHandler>()
        .AddHttpMessageHandler<SetAuthHeaderHandler>()
        .AddHttpMessageHandler<UnsuccessfulStatusCodeHandler>()
        .AddHttpMessageHandler<RefreshTokenHandler>()
        .AddHttpMessageHandler<UnhandeledExceptionHandler>();

        builder.Services.AddHttpClient<RefreshTokenService>(httpClient =>
        {
            httpClient.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
        })
        .AddHttpMessageHandler<IncludeCookiesHandler>();

        return builder;
    }
}
