using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Extensions;

var builder = WebAssemblyHostBuilder
    .CreateDefault(args)
    .AddRootComponents()
    .AddClientServices();

await builder.Build().RunAsync();