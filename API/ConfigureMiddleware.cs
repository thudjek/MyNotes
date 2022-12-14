using Application.Common.Exceptions;
using Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using SharedModels;

namespace API;

public static class ConfigureMiddleware
{
    public static async Task ConfigureMiddlewarePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            await app.InitilazeDatabaseForDevelopment();
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
