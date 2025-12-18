using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MinimalEshop.Presentation.Responses;
using System.Threading.Tasks;

public class AuthResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthResponseMiddleware> _logger;

    public AuthResponseMiddleware(
        RequestDelegate next,
        ILogger<AuthResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.HasStarted)
            return;

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            _logger.LogWarning(
                "Forbidden (403): User tried to access {Path} but does not have permission.",
                context.Request.Path);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(
                Result.Fail(null,
                    "You are not authorized to access this resource.",
                    StatusCodes.Status403Forbidden));
        }
        else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            _logger.LogWarning(
                "Unauthorized (401): User attempted to access {Path} without authentication.",
                context.Request.Path);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(
                Result.Fail(null,
                    "Authentication is required.",
                    StatusCodes.Status401Unauthorized));
        }
    }
}
