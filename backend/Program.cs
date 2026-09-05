using GraphForge.Api.Auth;
using GraphForge.Api.Database;
using GraphForge.Api.Models;
using GraphForge.Api.Services;
using GraphForge.Api.Services.AuthService;
using GraphForge.Api.Services.GraphService;
using GraphForge.Api.Services.ProjectService;
using GraphForge.Api.Services.UserIdentityProviderService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
AddAuth(builder);


string frontendUrl = builder.Configuration["FRONTEND_URL"]
    ?? throw new InvalidOperationException(
        "FRONTEND_URL is not configured"
    );


builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention()
);


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IGraphsService, GraphsService>();
builder.Services.AddScoped<IUserIdentityProvider, UserIdentityProvider>();

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?
            .Error;

        context.Response.ContentType = "application/problem+json";

        if (exception is NotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = exception.Message
            });

            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal server error",
            Detail = "Unexpected server error"
        });
    });
});


app.UseRouting();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void AddAuth(WebApplicationBuilder builder)
{
    var authOptions = new AuthOptions
    {
        Issuer = builder.Configuration["AUTH_ISSUER"]
            ?? throw new InvalidOperationException("AUTH_ISSUER is not configured"),

        Audience = builder.Configuration["AUTH_AUDIENCE"]
            ?? throw new InvalidOperationException("AUTH_AUDIENCE is not configured"),

        Key = builder.Configuration["JWT_KEY"]
            ?? throw new InvalidOperationException("JWT_KEY is not configured"),
    };

    builder.Services.AddSingleton(authOptions);

    var keyBytes = Encoding.UTF8.GetBytes(authOptions.Key);

    if (keyBytes.Length < 32)
    {
        throw new InvalidOperationException(
            "JWT_KEY must be at least 32 bytes long."
        );
    }

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = authOptions.Audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(authOptions.Key)
                )
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["access_token"];
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();
}
