using GraphForge.Api;
using GraphForge.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGraphForgeAuthentication();
builder.Services.AddGraphForgeCors(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddGraphForgeApi();
builder.Services.AddGraphForgeServices();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(ExceptionHandler.Handler);
});

app.UseRouting();
app.UseCors(CorsExtensions.FrontendCorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
