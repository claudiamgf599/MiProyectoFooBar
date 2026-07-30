using Azure.Core;
using FooBar.Domain.Exceptions;
using System.Net;

namespace FooBar.Api.Middleware;

public class AppExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AppExceptionHandlerMiddleware> _logger;
    private static readonly Dictionary<Type, HttpStatusCode> StatusCodes = new()
        {
            { typeof(CoreBusinessException), HttpStatusCode.BadRequest },
            { typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized }
        };

    public AppExceptionHandlerMiddleware(RequestDelegate next, ILogger<AppExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Domain.Exceptions.CoreBusinessException ex)
        {
            _logger.LogError(ex, "Business error: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { Error = ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Unauthorized error: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { Error = ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { Error = "An unexpected error occurred." }));
        }
    }
}
