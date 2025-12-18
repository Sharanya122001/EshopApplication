using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MinimalEshop.Presentation.Responses;
using Serilog;

public static class GlobalExceptionHandlerExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;

                // Logging
                Log.Error(ex,
                    "Unhandled exception occurred at path: {Path}. Message: {Message}",
                    feature?.Path,
                    ex?.Message);

                var result = Result.Fail(
                    new[] { ex?.Message ?? "An unexpected error occurred." },
                    "An error occurred while processing your request.",
                    500
                );

                await context.Response.WriteAsJsonAsync(result);
            });
        });
    }
}
