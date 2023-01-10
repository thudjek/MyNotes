using Application;
using Application.Common.Exceptions;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Serilog.Events;
using SharedModels;

namespace API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureBuilderAndServices(this WebApplicationBuilder builder)
    {
        builder.AddSerilog();

        builder.Services
            .AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration)
            .AddAPIServices(builder.Configuration);

        return builder;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.InitilazeDatabaseForDevelopment().Wait();
        }
        else
        {
            app.UseHsts();
            app.UseApiDomainAndHttpsScheme();
        }

        app.UseCors(policy =>
            policy.WithOrigins(app.Configuration["App:WebAppBaseUrl"])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseExceptionHandling();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    private static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));
    }

    private static void UseApiDomainAndHttpsScheme(this WebApplication app)
    {
        app.Use((context, next) =>
        {
            context.Request.Host = new HostString(app.Configuration["App:ApiDomain"]);
            context.Request.Scheme = "https";
            return next();
        });
    }

    private static async Task<WebApplication> InitilazeDatabaseForDevelopment(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await databaseInitializer.InitializeDatabase();
            await databaseInitializer.SeedAsync();
        }

        return app;
    }

    private static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(configure =>
        {
            configure.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                context.Response.ContentType = "application/json";

                var errorModel = new ErrorModel();

                switch (exceptionHandlerPathFeature?.Error)
                {
                    case ValidationException ex:
                        errorModel.Error = ex.Message;
                        errorModel.Errors = ex.Errors;
                        errorModel.ErrorsGrouped = ex.ErrorsGrouped;
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync(errorModel.ToJson());
                        break;
                    case NotFoundException ex:
                        if (!string.IsNullOrWhiteSpace(ex.UserMessage))
                            errorModel.Error = ex.UserMessage;
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync(errorModel.ToJson());
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync(errorModel.ToJson());
                        break;
                }
            });
        });

        return app;
    }
}
