using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests;

public static class SchemaTestClientExtensions
{
    /// <summary>
    /// Creates authorized client, then creates project. Then adds one schema with empty fields colection to the project
    /// </summary>
    /// <returns>
    /// (
    /// <see cref="HttpClient"/> AuthorizedClient, 
    /// <see cref="ProjectInfoResponse"/> Project, 
    /// <see cref="SchemaResponse "/>Schema
    /// )
    /// </returns>
    public static async Task<(HttpClient Client, ProjectInfoResponse Project, SchemaResponse Schema)>
        CreateAuthorizedClientWithSchemaAsync(
            this ApiPostgresTestFactory factory,
            string schemaTypeName = "SchemaName",
            List<SchemaFieldCreationRequest>? fields = null)
    {
        (HttpClient client, ProjectInfoResponse project) =
            await factory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/schemas",
            new
            {
                schemaTypeName,
                fields = fields ?? new List<SchemaFieldCreationRequest>()
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        SchemaResponse? schema = await response.Content.ReadFromJsonAsync<SchemaResponse>();
        Assert.NotNull(schema);

        return (client, project, schema);
    }
}