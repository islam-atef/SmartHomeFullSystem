using API.Helper;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace API.Middlewares
{
    public class ExceptionsMiddleware
        (RequestDelegate _next, IWebHostEnvironment _WebhostingEnvironment, IMemoryCache _memoryCache)
    {
        private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ApplySecureHeaders(context);
                if (!IsRequestAllowed(context))
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests; // Too Many Requests
                    context.Response.ContentType = "application/json";
                    var response = new ApiException(context.Response.StatusCode, "Too many requests. Please try again later.");
                    var jsonResponse = JsonSerializer.Serialize(response);
                    return; // Request is not allowed, exit middleware
                }
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = _WebhostingEnvironment.IsDevelopment() ?
                    new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, ex.Message);

                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private bool IsRequestAllowed(HttpContext context)
        {
            var IpAddress = context.Connection.RemoteIpAddress?.ToString();
            var cachKey = $"RequestCount_{IpAddress}";
            var DateNow = DateTime.Now;

            var (timestamp, requestCount) = _memoryCache.GetOrCreate(cachKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
                return (timestamp: DateNow, requestCount: 0);
            });

            if (DateNow - timestamp < _rateLimitWindow)
            {
                if (requestCount < 100) // Allow up to 100 requests in the time window
                {
                    requestCount++;
                    _memoryCache.Set(cachKey, (timestamp, requestCount));
                    return true;
                }
                else
                    return false;
            }
            else
            {
                _memoryCache.Set(cachKey, (DateNow, requestCount)); // Reset count and timestamp
                return true;
            }
        }

        private void ApplySecureHeaders(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
        }
    }
}
