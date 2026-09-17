using Microsoft.AspNetCore.Mvc;

namespace GraphForge.Api.Configuration;

internal static class ApiExtensions
{
    public static IServiceCollection AddGraphForgeApi(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddProblemDetails();

        ConfigureValidationErrorResponses(services);

        return services;
    }

    private static void ConfigureValidationErrorResponses(IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .SelectMany(entry => entry.Value!.Errors.Select(error =>
                        $"{entry.Key}: {error.ErrorMessage}"
                    ));

                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation error",
                    Detail = string.Join("; ", errors)
                };

                return new BadRequestObjectResult(problem);
            };
        });
    }
}
