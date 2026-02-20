using AMZN.DTOs.Common;
using AMZN.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace AMZN.Extensions
{
    public static class ApiValidationExtensions
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
                        Code = ErrorCodes.ValidationError,
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
