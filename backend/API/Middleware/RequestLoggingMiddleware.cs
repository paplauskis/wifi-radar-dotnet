using System.Diagnostics;

namespace API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", 
                context.Request.Method, context.Request.Path);
            
            throw;
        }
        finally
        {
            var elapsedMs = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;

            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                           ?? context.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation(
                $"{DateTime.UtcNow} HTTP {context.Request.Method} {context.Request.Path}{context.Request.QueryString} " +
                $"IP: {ip} " +
                $"Response code: {context.Response.StatusCode}; Response time: {elapsedMs} ms"
            );
        }
    }
}
