using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace BillPaymentApi.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task Invoke(HttpContext context)
        {
            // The rule is only for the "Query Bill" endpoint, Admin or other endpoints should not be affected.
            if (context.Request.Path.StartsWithSegments("/api/v1/Subscriber/bills") &&
                !context.Request.Path.StartsWithSegments("/api/v1/Subscriber/bills/detailed") &&
                context.Request.Method == "GET")
            {
                // Get User ID (from Token)
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

                    // Cache Key: RateLimit_1_2024-11-22
                    var cacheKey = $"RateLimit_{userId}_{today}";

                    // Get current count from cache, 0 otherwise
                    _memoryCache.TryGetValue(cacheKey, out int currentCount);

                    if (currentCount >= 3)
                    {
                        // LIMIT EXCEEDED
                        context.Response.StatusCode = 429; // Too Many Requests
                        await context.Response.WriteAsJsonAsync(new { Error = "You have exceeded the daily limit of 3 queries." });
                        return; // Interrupt the request here, do not go to the Controller
                    }

                    // If it is not over the limit, increase the counter by 1 and keep it for 1 day
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.UtcNow.AddDays(1) // Delete the next day
                    };

                    _memoryCache.Set(cacheKey, currentCount + 1, cacheOptions);
                }
            }

            // Go to the next step
            await _next(context);
        }
    }
}