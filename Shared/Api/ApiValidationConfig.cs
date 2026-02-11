using AMZN.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace AMZN.Shared.Api
{
    public static class ApiValidationConfig
    {
        public static IServiceCollection AddApiValidationErrors(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );


                    return new BadRequestObjectResult(new ApiErrorResponse
                    {
                        Code = "validation.error",
                        Message = "Validation failed",
                        TraceId = context.HttpContext.TraceIdentifier,
                        Errors = errors
                    });

                };

            });

            return services;
        }
    }
}
