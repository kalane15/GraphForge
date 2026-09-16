using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests;

public static class GraphWithSchemaTestClientExtensions
{
    /// <summary>
    /// Creates authorized client, project, schema and empty graph, then returns all created resources.
    /// Use this when a test needs the schema id to build or validate graph content explicitly.
    /// </summary>
    public static async Task<(HttpClient Client, ProjectInfoResponse Project, GraphInfoResponse Graph, SchemaResponse Schema)>
        CreateAuthorizedClientWithGraphAndSchemaAsync(
            this ApiPostgresTestFactory factory,
            string graphName = "GraphName")
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse schema) =
            await factory.CreateAuthorizedClientWithSchemaAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{project.Id}/graphs", new
        {
            name = graphName
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GraphInfoResponse? graph = await response.Content.ReadFromJsonAsync<GraphInfoResponse>();
        Assert.NotNull(graph);

        return (client, project, graph, schema);
    }
}
