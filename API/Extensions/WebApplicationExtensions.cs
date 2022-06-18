using API.Models;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
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
                        errorModel.Message = ex.Message;
                        errorModel.Errors = ex.Errors;
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync(errorModel.ToJson());
                        break;
                    case NotFoundException ex:
                        errorModel.Message = ex.Message;
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
