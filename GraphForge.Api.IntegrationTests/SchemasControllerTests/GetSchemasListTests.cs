using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.SchemasControllerTests;

public sealed class GetSchemasListTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public GetSchemasListTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task GetSchemasList_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{project.Id}/schemas"));
    }


    [Fact]
    public async Task GetSchemasList_WhenProjectBelongsToOtherUser_Returns404ProblemDetails()
    {
        (_, ProjectInfoResponse project) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{project.Id}/schemas"));
    }


    [Fact]
    public async Task GetSchemasList_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.GetAsync($"/api/projects/{Guid.NewGuid()}/schemas");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task GetSchemasList_WhenProjectEmpty_Returns200WithZeroSchemas()
    {
        (HttpClient client, ProjectInfoResponse project) 
            = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.GetAsync($"/api/projects/{project.Id}/schemas");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        SchemasListResponse? schemasList = await response.Content.ReadFromJsonAsync<SchemasListResponse>();
        Assert.NotNull(schemasList);

        Assert.Empty(schemasList.Schemas);
    }



    [Fact]
    public async Task GetSchemasList_WhenProjectContainsSchemas_Returns200WithSameSchemas()
    {
        (HttpClient client, ProjectInfoResponse project)
            = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        var expectedSchemas = new List<SchemaCreateRequest>();

        for (int i = 0; i < 10; i++)
        {
            SchemaCreateRequest schema = new SchemaCreateRequest($"Schema{i}", RandomFieldsFactory.GetFieldsRandomValidData());

            expectedSchemas.Add(schema);

            HttpResponseMessage createResponse = await client.PostAsJsonAsync($"/api/projects/{project.Id}/schemas", schema);

            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        }


        HttpResponseMessage response = await client.GetAsync($"/api/projects/{project.Id}/schemas");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        SchemasListResponse? schemasList = await response.Content.ReadFromJsonAsync<SchemasListResponse>();

        Assert.NotNull(schemasList);
        Assert.Equal(expectedSchemas.Count, schemasList.Schemas.Count);

        foreach (SchemaCreateRequest expectedSchema in expectedSchemas)
        {
            SchemaResponse? actualSchema = schemasList.Schemas.FirstOrDefault(
                    (schema) => schema.SchemaTypeName == expectedSchema.SchemaTypeName
                );

            Assert.NotNull(actualSchema);
            AssertSchemaFieldsEqual(expectedSchema.Fields, actualSchema.Fields);
        }        
    }
}
