using Serilog;
using System.Diagnostics;

namespace FlowingBot.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Log.ForContext<RequestLoggingMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;
            var requestId = Guid.NewGuid().ToString();

            try
            {
                _logger.Information("Request {RequestId} started - {Method} {Path} from {IP}", 
                    requestId, 
                    requestMethod, 
                    requestPath, 
                    GetClientIpAddress(context));

                await _next(context);

                stopwatch.Stop();
                _logger.Information("Request {RequestId} completed - {Method} {Path} - Status: {StatusCode} in {ElapsedMs}ms", 
                    requestId, 
                    requestMethod, 
                    requestPath, 
                    context.Response.StatusCode, 
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error(ex, "Request {RequestId} failed - {Method} {Path} after {ElapsedMs}ms", 
                    requestId, 
                    requestMethod, 
                    requestPath, 
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private string GetClientIpAddress(HttpContext context)
        {
            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
                return forwardedHeader.Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }

    // Extension method for easy registration
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder) =>
            builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}