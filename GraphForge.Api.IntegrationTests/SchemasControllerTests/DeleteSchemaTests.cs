using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.SchemasControllerTests;

public sealed class DeleteSchemaTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public DeleteSchemaTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task DeleteSchema_WhenSchemaExists_Returns204AndGetReturns404()
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();


        HttpResponseMessage response = await client.DeleteAsync($"/api/projects/{project.Id}/schemas/{schema.Id}");


        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        response = await client.GetAsync($"/api/projects/{project.Id}/schemas/{schema.Id}");

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteSchema_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.DeleteAsync($"/api/projects/{project.Id}/schemas/{schema.Id}"));
    }


    [Fact]
    public async Task DeleteSchema_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.DeleteAsync($"/api/projects/{Guid.NewGuid()}/schemas/{Guid.NewGuid()}");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteSchema_WhenSchemaDoesNotExist_Returns404ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.DeleteAsync($"/api/projects/{info.Id}/schemas/{Guid.NewGuid()}");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteSchema_WhenSchemaBelongsToOtherUser_Returns404ProblemDetails()
    {
        (HttpClient ownerClient, ProjectInfoResponse project, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();


        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.DeleteAsync($"/api/projects/{project.Id}/schemas/{schema.Id}"));

        HttpResponseMessage getResponse = await ownerClient.GetAsync($"/api/projects/{project.Id}/schemas/{schema.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        SchemaResponse? existingSchema = await getResponse.Content.ReadFromJsonAsync<SchemaResponse>();
        Assert.NotNull(existingSchema);
        Assert.Equal(schema.Id, existingSchema.Id);
    }
}
