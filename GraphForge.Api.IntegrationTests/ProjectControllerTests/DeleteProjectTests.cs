using System.Net;
using GraphForge.Api.DTOs.Projects;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;

public sealed class DeleteProjectTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    public DeleteProjectTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }

    [Fact]
    public async Task DeleteProject_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.DeleteAsync($"api/projects/{Guid.NewGuid()}");

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProject_WhenProjectExist_Returns204AndGetReturns404()
    {
        (HttpClient client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpResponseMessage deleteResponse = await client.DeleteAsync($"api/projects/{info.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        HttpResponseMessage getResponse = await client.GetAsync($"api/projects/{info.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_WhenProjectBelongsToOtherUser_Returns404ProblemDetails()
    {
        (_, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.DeleteAsync($"api/projects/{info.Id}"));
    }

    [Fact]
    public async Task DeleteProject_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.DeleteAsync($"api/projects/{info.Id}"));
    }
}

