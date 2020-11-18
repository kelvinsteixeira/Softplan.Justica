using System;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Softplan.Justica.GerenciadorProcessos.Models;

namespace Softplan.Justica.GerenciadorProcessos.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;

                    ErrorResponseWrapper<string> error = new ErrorResponseWrapper<string>();
                    switch (exception)
                    {
                        case FormatException formatException:
                            error.ErrorMessage = formatException.Message;
                            break;

                        default:
                            error.ErrorMessage = "Erro inesperado.";
                            break;
                    }

                    var result = JsonSerializer.Serialize(error);
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    await context.Response.WriteAsync(result);
                });
            });

            return app;
        }
    }
}