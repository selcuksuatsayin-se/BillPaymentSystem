using BillPaymentApi.Data;
using BillPaymentApi.Entities;
using System.Diagnostics;

namespace BillPaymentApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public RequestLoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            // 1. Start time measurement
            var stopwatch = Stopwatch.StartNew();
            var requestTime = DateTime.UtcNow;

            // Enable buffering to read the request body
            context.Request.EnableBuffering();

            // Keep original Response Body stream
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                try
                {
                    // --> Send the request to the next step (Controller)
                    await _next(context);
                }
                catch (Exception)
                {
                    // If there is an error, we will log it and throw it
                    throw;
                }
                finally
                {
                    // 2. Process completed, stop time
                    stopwatch.Stop();

                    var headers = string.Join("; ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"));
                    // Prepare log data
                    var log = new ApiLog
                    {
                        HttpMethod = context.Request.Method,
                        Path = context.Request.Path,
                        RequestTime = requestTime,
                        ResponseLatencyMs = stopwatch.ElapsedMilliseconds,
                        StatusCode = context.Response.StatusCode,
                        IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                        RequestHeaders = headers,
                        RequestSize = context.Request.ContentLength ?? 0,
                        ResponseSize = responseBody.Length,
                        IsAuthenticated = context.User.Identity?.IsAuthenticated ?? false
                    };

                    // 3. Save to Database (using Scope)
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        dbContext.ApiLogs.Add(log);
                        await dbContext.SaveChangesAsync();
                    }

                    // Copy the response back to the original stream so see the response
                    responseBody.Position = 0;
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }
    }
}