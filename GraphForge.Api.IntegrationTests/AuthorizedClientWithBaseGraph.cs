using System.Net;
using System.Net.Http.Json;
using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;
using GraphForge.Contracts;

namespace GraphForge.Api.IntegrationTests;

public static class GraphTestClientExtensions
{
    /// <summary>
    /// Creates authorized client, project, schema and graph with valid base content.
    /// </summary>
    public static async Task<(HttpClient, ProjectInfoResponse, GraphInfoResponse)> CreateAuthorizedClientWithBaseGraphAsync(
        this ApiPostgresTestFactory factory,
        string graphName = "GraphName")
    {
        (HttpClient client, ProjectInfoResponse project, GraphInfoResponse graph, _) =
            await factory.CreateAuthorizedClientWithBaseGraphContentAsync(graphName);

        return (client, project, graph);
    }

    /// <summary>
    /// Creates authorized client, project, schema and graph with valid base content, then returns that content for assertions.
    /// </summary>
    public static async Task<(HttpClient, ProjectInfoResponse, GraphInfoResponse, GraphDto)> CreateAuthorizedClientWithBaseGraphContentAsync(
        this ApiPostgresTestFactory factory,
        string graphName = "GraphName")
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse schema) =
            await factory.CreateAuthorizedClientWithSchemaAsync();

        GraphDto validGraph = GraphDtoFactory.CreateValidGraph(schema.Id);

        HttpResponseMessage response = await client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/graphs",
            new
            {
                name = graphName
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GraphInfoResponse? graph = await response.Content.ReadFromJsonAsync<GraphInfoResponse>();
        Assert.NotNull(graph);

        response = await client.PutAsJsonAsync(
            $"/api/projects/{project.Id}/graphs/{graph.Id}/content",
            new
            {
                content = validGraph
            });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        return (client, project, graph, validGraph);
    }
}
