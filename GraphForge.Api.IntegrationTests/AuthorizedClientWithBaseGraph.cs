using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;
using GraphForge.Contracts;
using System.Net;
using System.Net.Http.Json;

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
        (HttpClient client, ProjectInfoResponse project, SchemaResponse schema) =
            await factory.CreateAuthorizedClientWithSchemaAsync();

        string firstNodeId = Guid.NewGuid().ToString();
        string secondNodeId = Guid.NewGuid().ToString();

        GraphDto validGraph = new GraphDtoBuilder()
            .WithNode(firstNodeId, schema.Id)
            .WithNode(secondNodeId, schema.Id)
            .WithEdge(source: firstNodeId, target: secondNodeId)
            .Build();

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

        return (client, project, graph);
    }
}
