using Application;
using Infrastructure;
using Serilog;
using Serilog.Events;
using API.Extensions;
using API;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddAPIServices(builder.Configuration);

    var app = builder.Build();

    await app.ConfigureMiddlewarePipeline();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to start");
}
finally
{
    Log.Information("Shutting down");
    Log.CloseAndFlush();
}
