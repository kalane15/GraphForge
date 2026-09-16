using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace GraphForge.Api.IntegrationTests;

internal static class ProblemDetailsAssert
{
    public static async Task<ProblemDetails> AssertProblemDetailsAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatusCode)
    {
        Assert.Equal(expectedStatusCode, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal((int)expectedStatusCode, problem.Status);

        return problem;
    }
}
