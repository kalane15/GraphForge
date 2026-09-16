using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;
using System.Net;

namespace GraphForge.Api.IntegrationTests.GraphControllerTests;

public sealed class DeleteGraphTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public DeleteGraphTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task DeleteGraph_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project, GraphInfoResponse graph) =
            await _apiTestFactory.CreateAuthorizedClientWithBaseGraphAsync();

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.DeleteAsync($"/api/projects/{project.Id}/graphs/{graph.Id}"));
    }


    [Fact]
    public async Task DeleteGraph_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.DeleteAsync($"/api/projects/{Guid.NewGuid()}/graphs/{Guid.NewGuid()}");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteGraph_WhenGraphDoesNotExist_Returns404ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.DeleteAsync($"/api/projects/{project.Id}/graphs/{Guid.NewGuid()}");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteGraph_WhenGraphBelongsToOtherUser_Returns404ProblemDetails()
    {
        (HttpClient ownerClient, ProjectInfoResponse project, GraphInfoResponse graph) =
            await _apiTestFactory.CreateAuthorizedClientWithBaseGraphAsync();

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.DeleteAsync($"/api/projects/{project.Id}/graphs/{graph.Id}"));

        HttpResponseMessage getResponse = await ownerClient.GetAsync($"/api/projects/{project.Id}/graphs/{graph.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }


    [Fact]
    public async Task DeleteGraph_WhenGraphExists_Returns204AndGetReturns404()
    {
        (HttpClient client, ProjectInfoResponse project, GraphInfoResponse graph) =
            await _apiTestFactory.CreateAuthorizedClientWithBaseGraphAsync();


        HttpResponseMessage deleteResponse = await client.DeleteAsync($"/api/projects/{project.Id}/graphs/{graph.Id}");


        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        HttpResponseMessage getResponse = await client.GetAsync($"/api/projects/{project.Id}/graphs/{graph.Id}");

        await AssertProblemDetailsAsync(getResponse, HttpStatusCode.NotFound);
    }
}
