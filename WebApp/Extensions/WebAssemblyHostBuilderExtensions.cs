using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Auth;
using WebApp.Common;
using WebApp.Services;

namespace WebApp.Extensions;

public static class WebAssemblyHostBuilderExtensions
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
        builder.Services.AddTransient<InterceptingHttpRequestHandler>();
        builder.Services.AddTransient<RefreshTokenService>();

        builder.Services.AddHttpClient(Constants.HttpClientName, httpClient => 
        {
            httpClient.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
        })
        .AddHttpMessageHandler<InterceptingHttpRequestHandler>();

        return builder;
    }
}
