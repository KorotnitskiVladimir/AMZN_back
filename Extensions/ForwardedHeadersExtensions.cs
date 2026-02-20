using Microsoft.AspNetCore.HttpOverrides;



namespace AMZN.Extensions
{
    public static class ForwardedHeadersExtensions
    {
        public static IServiceCollection AddAmznForwardedHeaders(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;

                options.ForwardLimit = 2;

                // В Azure/AWS/Kubernetes  ->
                // Если RemoteIpAddress всегда одинаковый (IP прокси) -> значит приложение за reverse proxy и реальный IP в X-Forwarded-For.
                // Тогда нужно настроить доверенные прокси под конкретный хостинг
                // См. https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer
            });

            return services;
        }


        public static WebApplication UseAmznForwardedHeaders(this WebApplication app)
        {
            app.UseForwardedHeaders();
            return app;
        }

    }
}

