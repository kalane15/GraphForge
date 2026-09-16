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
    /// Created authorized client and creates project 
    /// with name <paramref name="inputName"/> and description <paramref name="inputDescription"/>
    /// </summary>
    /// <returns>Returns (HttpClient, Created project DTO)</returns>
    public static async Task<(HttpClient, ProjectInfoResponse, GraphInfoResponse)> CreateAuthorizedClientWithBaseGraphAsync(
        this ApiPostgresTestFactory factory, string? inputName=null, string? inputDescription=null)
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse schema) =
                   await factory.CreateAuthorizedClientWithSchemaAsync();

        string firstNodeId = Guid.NewGuid().ToString();
        string secondNodeId = Guid.NewGuid().ToString();

        GraphDto validGraph = new GraphDtoBuilder()
            .WithNode(firstNodeId, schema.Id)
            .WithNode(secondNodeId, schema.Id)
            .WithEdge(firstNodeId, secondNodeId)
            .Build();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/graphs", validGraph);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GraphInfoResponse? graph = await response.Content.ReadFromJsonAsync<GraphInfoResponse>();
        Assert.NotNull(graph);

        return (client, project, graph);
    }
}
