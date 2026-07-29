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

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCodeForException(ex);
            
            // Serialize to bytes and write directly to avoid PipeWriter issues in .NET 10
            var errorResponse = new { ErrorMessage = ex.Message };
            var jsonBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(errorResponse);
            await context.Response.Body.WriteAsync(jsonBytes);
        }
    }

    private int GetStatusCodeForException(Exception ex)
    {
        return StatusCodes.TryGetValue(ex.GetType(), out var statusCode)
            ? (int)statusCode
            : (int)HttpStatusCode.InternalServerError;
    }
}
