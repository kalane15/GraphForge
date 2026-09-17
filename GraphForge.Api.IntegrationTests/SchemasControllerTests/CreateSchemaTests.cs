using System.Net;
using System.Net.Http.Json;
using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;

namespace GraphForge.Api.IntegrationTests.SchemasControllerTests;

public sealed class CreateSchemaTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    private const string SchemaDefaultTypeName = "SchemaName";

    public CreateSchemaTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }

    [Theory]
    [InlineData(SchemaDefaultTypeName)]
    public async Task CreateSchema_WhenDataCorrectEmptyFields_Returns200WithSameData(string schemaTypeName)
    {
        (HttpClient client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
        {
            schemaTypeName,
            fields = new List<SchemaFieldCreationRequest>()
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        SchemaResponse? dto = await response.Content.ReadFromJsonAsync<SchemaResponse>();

        Assert.NotNull(dto);

        Assert.Equal(schemaTypeName, dto.SchemaTypeName);
        Assert.Empty(dto.Fields);
    }

    [Theory]
    [InlineData(SchemaDefaultTypeName)]
    public async Task CreateSchema_WhenDataCorrectNotEmptyFields_Returns200WithSameData(string schemaTypeName)
    {
        (HttpClient client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        List<SchemaFieldCreationRequest> expectedFields = RandomFieldsFactory.GetFieldsRandomValidData();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
        {
            schemaTypeName,
            fields = expectedFields
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        SchemaResponse? dto = await response.Content.ReadFromJsonAsync<SchemaResponse>();

        Assert.NotNull(dto);

        Assert.Equal(schemaTypeName, dto.SchemaTypeName);

        AssertSchemaFieldsEqual(expectedFields, dto.Fields);
    }

    [Theory]
    [InlineData(SchemaDefaultTypeName)]
    public async Task CreateSchema_WhenNotAuthorized_Returns401ProblemDetails(string schemaTypeName)
    {
        (_, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
            {
                schemaTypeName,
                fields = RandomFieldsFactory.GetFieldsRandomValidData()
            }));
    }

    [Theory]
    [InlineData(SchemaDefaultTypeName)]
    public async Task CreateSchema_WhenProjectDoesNotExist_Returns404ProblemDetails(string schemaTypeName)
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{Guid.NewGuid()}/schemas", new
        {
            schemaTypeName,
            fields = RandomFieldsFactory.GetFieldsRandomValidData()
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(SchemaDefaultTypeName)]
    public async Task CreateSchema_WhenProjectBelongsToOtherUser_Returns404ProblemDetails(string schemaTypeName)
    {
        (_, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
            {
                schemaTypeName,
                fields = RandomFieldsFactory.GetFieldsRandomValidData()
            }));
    }

    [Fact]
    public async Task CreateSchema_WhenNameIsMissing_Returns400ProblemDetails()
    {
        (var client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
        {
            fields = RandomFieldsFactory.GetFieldsRandomValidData()
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(SchemaDefaultTypeName)]
    public async Task CreateSchema_WhenFieldsAreMissing_Returns400ProblemDetails(string schemaTypeName)
    {
        (var client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
        {
            schemaTypeName
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(SchemaDefaultTypeName)]
    public async Task CreateSchema_WhenFieldsAreNull_Returns400ProblemDetails(string schemaTypeName)
    {
        (var client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
        {
            schemaTypeName,
            fields = (List<SchemaFieldCreationRequest>?)null
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(SchemaDefaultTypeName)]
    public async Task CreateSchema_WhenSchemaSameNameExists_Returns400ProblemDetails(string schemaTypeName)
    {
        (var client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
        {
            schemaTypeName,
            fields = RandomFieldsFactory.GetFieldsRandomValidData()
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        response = await client.PostAsJsonAsync($"/api/projects/{info.Id}/schemas", new
        {
            schemaTypeName,
            fields = RandomFieldsFactory.GetFieldsRandomValidData()
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }
}
