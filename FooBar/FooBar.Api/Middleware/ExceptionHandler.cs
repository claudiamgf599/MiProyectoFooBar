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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: {Message}", ex.Message);

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                ErrorMessage = ex.Message
            });

            context.Response.ContentType = ContentType.ApplicationJson.ToString();
            context.Response.StatusCode = GetStatusCodeForException(ex);
            await context.Response.WriteAsync(result);
        }
    }

    private int GetStatusCodeForException(Exception ex)
    {
        return StatusCodes.TryGetValue(ex.GetType(), out var statusCode)
            ? (int)statusCode
            : (int)HttpStatusCode.InternalServerError;
    }
}
