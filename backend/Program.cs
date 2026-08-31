using GraphForge.Api.Auth;
using GraphForge.Api.Database;
using GraphForge.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();


string frontendUrl = builder.Configuration["FRONTEND_URL"]
    ?? throw new InvalidOperationException(
        "FRONTEND_URL is not configured"
    );

string jwtKey = builder.Configuration["JWT_KEY"]
        ?? throw new InvalidOperationException("JWT_KEY is not configured");

builder.Services.Configure<AuthOptions>(options =>
{
    options.Issuer = builder.Configuration["AUTH_ISSUER"]
        ?? throw new InvalidOperationException("AUTH_ISSUER is not configured");

    options.Audience = builder.Configuration["AUTH_AUDIENCE"]
        ?? throw new InvalidOperationException("AUTH_AUDIENCE is not configured");

    options.Key = jwtKey;
});

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

if (keyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "JWT_KEY must be at least 32 bytes long."
    );
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(frontendUrl!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();
app.UseRouting();
app.UseCors("Frontend");
app.MapControllers();
app.Run();
