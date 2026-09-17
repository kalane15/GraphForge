using System.Net;

namespace GraphForge.Api.IntegrationTests;

internal static class AuthorizationAssert
{
    /// <summary>
    /// Sends the request with an anonymous client and verifies that API authorization rejects it with 401 ProblemDetails.
    /// The request itself is passed from the test so the endpoint and payload remain visible in the test body.
    /// </summary>
    public static async Task AssertUnauthorizedAsync(
        ApiPostgresTestFactory factory,
        Func<HttpClient, Task<HttpResponseMessage>> sendRequest)
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await sendRequest(client);

        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Sends the request with a different authorized user and verifies that protected resources are hidden as 404 ProblemDetails.
    /// Use this when a test has already created a resource owned by another user and wants to check ownership isolation.
    /// </summary>
    public static async Task AssertAccessOtherUserResourceReturnsNotFoundAsync(
        ApiPostgresTestFactory factory,
        Func<HttpClient, Task<HttpResponseMessage>> sendRequest)
    {
        HttpClient client = await factory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await sendRequest(client);

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }
}
