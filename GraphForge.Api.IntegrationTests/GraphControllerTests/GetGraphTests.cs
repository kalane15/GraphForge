using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;
using GraphForge.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.GraphControllerTests;

public sealed class GetGraphTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public GetGraphTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task GetGraph_WhenGraphExistsAndAccessValid_Returns200WithCorrectData()
    {
        (HttpClient client, ProjectInfoResponse project, GraphInfoResponse graph, GraphDto expectedContent) =
            await _apiTestFactory.CreateAuthorizedClientWithBaseGraphContentAsync();


        HttpResponseMessage response = await client.GetAsync($"/api/projects/{project.Id}/graphs/{graph.Id}");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GraphDataResponse? result = await response.Content.ReadFromJsonAsync<GraphDataResponse>();
        Assert.NotNull(result);
        Assert.Equal(graph.Id, result.Id);
        Assert.Equal(project.Id, result.ProjectId);
        Assert.Equal(graph.Name, result.Name);

        AssertGraphDtoEqual(expectedContent, result.Content);
    }


    [Fact]
    public async Task GetGraph_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project, GraphInfoResponse graph) =
            await _apiTestFactory.CreateAuthorizedClientWithBaseGraphAsync();

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{project.Id}/graphs/{graph.Id}"));
    }


    [Fact]
    public async Task GetGraph_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.GetAsync($"/api/projects/{Guid.NewGuid()}/graphs/{Guid.NewGuid()}");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task GetGraph_WhenGraphDoesNotExist_Returns404ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.GetAsync($"/api/projects/{project.Id}/graphs/{Guid.NewGuid()}");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task GetGraph_WhenGraphBelongsToOtherUser_Returns404ProblemDetails()
    {
        (_, ProjectInfoResponse project, GraphInfoResponse graph) =
            await _apiTestFactory.CreateAuthorizedClientWithBaseGraphAsync();

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{project.Id}/graphs/{graph.Id}"));
    }
}
