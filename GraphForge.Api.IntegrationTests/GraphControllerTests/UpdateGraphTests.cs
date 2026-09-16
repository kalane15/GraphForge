using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;
using GraphForge.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.GraphControllerTests;

public sealed class UpdateGraphTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public UpdateGraphTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task UpdateGraph_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project, GraphInfoResponse graph, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync();

        GraphDto content = GraphDtoFactory.CreateValidGraph(schema.Id);

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.PutAsJsonAsync($"/api/projects/{project.Id}/graphs/{graph.Id}", new
            {
                name = "UpdatedGraph",
                content
            }));
    }


    [Fact]
    public async Task UpdateGraph_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        (HttpClient client, _, _, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{Guid.NewGuid()}/graphs/{Guid.NewGuid()}", new
        {
            name = "UpdatedGraph",
            content = GraphDtoFactory.CreateValidGraph(schema.Id)
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task UpdateGraph_WhenGraphDoesNotExist_Returns404ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, _, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/graphs/{Guid.NewGuid()}", new
        {
            name = "UpdatedGraph",
            content = GraphDtoFactory.CreateValidGraph(schema.Id)
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task UpdateGraph_WhenGraphBelongsToOtherUser_Returns404ProblemDetails()
    {
        (HttpClient ownerClient, ProjectInfoResponse project, GraphInfoResponse graph, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync(graphName: "OldGraph");

        GraphDto content = GraphDtoFactory.CreateValidGraph(schema.Id);

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.PutAsJsonAsync($"/api/projects/{project.Id}/graphs/{graph.Id}", new
            {
                name = "UpdatedGraph",
                content
            }));

        HttpResponseMessage getResponse = await ownerClient.GetAsync($"/api/projects/{project.Id}/graphs/{graph.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        GraphDataResponse? actualGraph = await getResponse.Content.ReadFromJsonAsync<GraphDataResponse>();
        Assert.NotNull(actualGraph);
        Assert.Equal("OldGraph", actualGraph.Name);
    }


    [Fact]
    public async Task UpdateGraph_WhenNameIsMissing_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, GraphInfoResponse graph, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/graphs/{graph.Id}", new
        {
            content = GraphDtoFactory.CreateValidGraph(schema.Id)
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateGraph_WhenContentIsMissing_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, GraphInfoResponse graph, _) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/graphs/{graph.Id}", new
        {
            name = "UpdatedGraph"
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateGraph_WhenNameIsWhitespace_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, GraphInfoResponse graph, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/graphs/{graph.Id}", new
        {
            name = "   ",
            content = GraphDtoFactory.CreateValidGraph(schema.Id)
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateGraph_WhenContentInvalid_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, GraphInfoResponse graph, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync();

        GraphDto invalidContent = new GraphDtoBuilder()
            .WithNode("same-id", schema.Id)
            .WithNode("same-id", schema.Id)
            .Build();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/graphs/{graph.Id}", new
        {
            name = "UpdatedGraph",
            content = invalidContent
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateGraph_WhenDataCorrect_Returns200WithNewDataAndPersistsChanges()
    {
        string newName = "UpdatedGraph";
        (HttpClient client, ProjectInfoResponse project, GraphInfoResponse graph, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithGraphAndSchemaAsync();

        GraphDto updatedContent = GraphDtoFactory.CreateValidGraph(schema.Id);


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/graphs/{graph.Id}", new
        {
            name = newName,
            content = updatedContent
        });


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GraphDataResponse? updateResult = await response.Content.ReadFromJsonAsync<GraphDataResponse>();
        Assert.NotNull(updateResult);
        Assert.Equal(graph.Id, updateResult.Id);
        Assert.Equal(project.Id, updateResult.ProjectId);
        Assert.Equal(newName, updateResult.Name);

        AssertGraphDtoEqual(updatedContent, updateResult.Content);

        HttpResponseMessage getResponse = await client.GetAsync($"/api/projects/{project.Id}/graphs/{graph.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        GraphDataResponse? actualGraph = await getResponse.Content.ReadFromJsonAsync<GraphDataResponse>();
        Assert.NotNull(actualGraph);
        Assert.Equal(newName, actualGraph.Name);

        AssertGraphDtoEqual(updatedContent, actualGraph.Content);
    }

}
