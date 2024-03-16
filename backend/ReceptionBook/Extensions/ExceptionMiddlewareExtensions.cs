using Contracts;
using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace ReceptionBook.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        BadRequestException => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    var isServerError = context.Response.StatusCode == StatusCodes.Status500InternalServerError;

                    // Expected client errors are logged without a stack trace to keep the logs readable.
                    if (isServerError)
                        logger.LogError($"Something went wrong: {contextFeature.Error}");
                    else
                        logger.LogWarn($"{context.Request.Method} {context.Request.Path} failed: {contextFeature.Error.Message}");

                    // Do not expose internal error details to the client.
                    var message = isServerError ? "Internal Server Error." : contextFeature.Error.Message;

                    await context.Response.WriteAsync(new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = message
                    }.ToString());
                }
            });
        });
    }
}
