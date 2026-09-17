using System.Net;
using System.Net.Http.Json;
using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;

namespace GraphForge.Api.IntegrationTests.SchemasControllerTests;

public sealed class GetSchemaTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public GetSchemaTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }

    [Fact]
    public async Task GetSchema_WhenSchemaExistsAndAccessValid_Returns200WithCorrectData()
    {
        List<SchemaFieldCreationRequest> expectedFields =
        [
            new("text", "string"),
            new("count", "int")
        ];

        (HttpClient client, ProjectInfoResponse project, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync(fields: expectedFields);

        HttpResponseMessage response = await client.GetAsync($"/api/projects/{project.Id}/schemas/{schema.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        SchemaResponse? result = await response.Content.ReadFromJsonAsync<SchemaResponse>();
        Assert.NotNull(result);
        Assert.Equal(schema.Id, result.Id);
        Assert.Equal(schema.SchemaTypeName, result.SchemaTypeName);
        AssertSchemaFieldsEqual(expectedFields, result.Fields);
    }

    [Fact]
    public async Task GetSchema_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{project.Id}/schemas/{schema.Id}"));
    }

    [Fact]
    public async Task GetSchema_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.GetAsync($"/api/projects/{Guid.NewGuid()}/schemas/{Guid.NewGuid()}");

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSchema_WhenSchemaDoesNotExist_Returns404ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();

        HttpResponseMessage response = await client.GetAsync($"/api/projects/{project.Id}/schemas/{Guid.NewGuid()}");

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSchema_WhenSchemaBelongsToOtherUser_Returns404ProblemDetails()
    {
        (_, ProjectInfoResponse project, SchemaResponse schema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{project.Id}/schemas/{schema.Id}"));
    }
}
