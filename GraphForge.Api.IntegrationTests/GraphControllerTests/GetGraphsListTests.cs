using System.Net;
using System.Net.Http.Json;
using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;

namespace GraphForge.Api.IntegrationTests.GraphControllerTests;

public sealed class GetGraphsListTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public GetGraphsListTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }

    [Fact]
    public async Task GetGraphsList_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{project.Id}/graphs"));
    }

    [Fact]
    public async Task GetGraphsList_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.GetAsync($"/api/projects/{Guid.NewGuid()}/graphs");

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGraphsList_WhenProjectBelongsToOtherUser_Returns404ProblemDetails()
    {
        (_, ProjectInfoResponse project) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{project.Id}/graphs"));
    }

    [Fact]
    public async Task GetGraphsList_WhenProjectEmpty_Returns200WithZeroGraphs()
    {
        (HttpClient client, ProjectInfoResponse project) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpResponseMessage response = await client.GetAsync($"/api/projects/{project.Id}/graphs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GraphsListResponse? graphsList = await response.Content.ReadFromJsonAsync<GraphsListResponse>();
        Assert.NotNull(graphsList);

        Assert.Empty(graphsList.Graphs);
    }

    [Fact]
    public async Task GetGraphsList_WhenProjectContainsGraphs_Returns200WithSameGraphs()
    {
        (HttpClient client, ProjectInfoResponse project) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        var expectedNames = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            string name = $"Graph-{Guid.NewGuid():N}";
            expectedNames.Add(name);

            HttpResponseMessage createResponse = await client.PostAsJsonAsync($"/api/projects/{project.Id}/graphs", new
            {
                name
            });

            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        }

        HttpResponseMessage response = await client.GetAsync($"/api/projects/{project.Id}/graphs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GraphsListResponse? graphsList = await response.Content.ReadFromJsonAsync<GraphsListResponse>();
        Assert.NotNull(graphsList);
        Assert.Equal(expectedNames.Count, graphsList.Graphs.Count);

        foreach (string expectedName in expectedNames)
        {
            GraphInfoResponse? actualGraph = graphsList.Graphs.FirstOrDefault(graph => graph.Name == expectedName);

            Assert.NotNull(actualGraph);
            Assert.Equal(project.Id, actualGraph.ProjectId);
            Assert.NotEqual(Guid.Empty, actualGraph.Id);
            Assert.NotEqual(default, actualGraph.CreatedAt);
            Assert.NotEqual(default, actualGraph.UpdatedAt);
        }
    }
}
