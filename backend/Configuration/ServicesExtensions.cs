using GraphForge.Api.Services.AuthService;
using GraphForge.Api.Services.GraphService;
using GraphForge.Api.Services.ProjectService;
using GraphForge.Api.Services.SchemasService;
using GraphForge.Api.Services.UserIdentityProviderService;
using GraphForge.Validation;
using GraphForge.Validation.GraphValidationService;

namespace GraphForge.Api.Configuration;

internal static class ServicesExtensions
{
    public static IServiceCollection AddGraphForgeServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IAuthCookieService, AuthCookieService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProjectsService, ProjectsService>();
        services.AddScoped<IGraphsService, GraphsService>();
        services.AddScoped<IGraphJsonValidatorService, GraphJsonValidatorService>();
        services.AddScoped<ISchemaDtoValidatorService, SchemaDtoValidatorService>();
        services.AddScoped<ISchemasService, SchemasService>();
        services.AddScoped<IUserIdentityProvider, UserIdentityProvider>();

        return services;
    }
}
