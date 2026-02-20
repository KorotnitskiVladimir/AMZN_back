using System.Threading.RateLimiting;

namespace AMZN.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddAmznRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("Auth", context =>
                {
                    // В проде за прокси (Azure/Ingress) RemoteIpAddress может быть IP прокси.
                    // Тогда лимит станет общим на всех. Решение: ForwardedHeaders + X-Forwarded-For.
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,                 // максимум 10 запросов
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,                   // не ставим запросы "в очередь", лишние сразу 429

                        // Если QueueLimit > 0, то это определяет порядок обработки очереди:
                        // OldestFirst - сначала самые старые запросы (справедливее для пользователей)
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
                });
            });

            return services;
        }


        public static WebApplication UseAmznRateLimiting(this WebApplication app)
        {
            app.UseRateLimiter();
            return app;
        }
    }

}

