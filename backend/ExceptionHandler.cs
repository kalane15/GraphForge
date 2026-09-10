using GraphForge.Api.Services;
using GraphForge.Api.Services.AuthService;
using GraphForge.Api.Services.GraphService;
using GraphForge.Api.Services.ProjectService;
using GraphForge.Validation.GraphValidationService;
using GraphForge.Validation.SchemaValidationService.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GraphForge.Api;

public static class ExceptionHandler
{
    public async static Task Handler(HttpContext context)
    {
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?
            .Error;

        context.Response.ContentType = "application/problem+json";

        ProblemDetails problem = exception switch
        {
            NotFoundException => CreateProblemDetails(
                StatusCodes.Status404NotFound,
                "Not found",
                exception.Message),

            GraphValidationException or 
            SchemaValidationException or
            ProjectValidationException => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Validation error",
                exception.Message),

            IncorrectProjectOwnerException => CreateProblemDetails(
                StatusCodes.Status403Forbidden,
                "Forbidden",
                exception.Message),

            UnauthorizedUserException => CreateProblemDetails(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                exception.Message),

            UserAlreadyExistsException => CreateProblemDetails(
                StatusCodes.Status409Conflict,
                "User already exists",
                exception.Message),

            _ => CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "Unexpected server error")
        };

        context.Response.StatusCode = problem.Status
            ?? StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static ProblemDetails CreateProblemDetails(
        int status,
        string title,
        string detail)
    {
        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };
    }
}
