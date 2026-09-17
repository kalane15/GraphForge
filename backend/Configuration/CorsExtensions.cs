namespace GraphForge.Api.Configuration;

internal static class CorsExtensions
{
    public const string FrontendCorsPolicyName = "Frontend";

    public static IServiceCollection AddGraphForgeCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicyName, policy =>
            {
                string frontendUrl = configuration["FRONTEND_URL"]
                    ?? throw new InvalidOperationException(
                        "FRONTEND_URL is not configured"
                    );

                policy
                    .WithOrigins(frontendUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
