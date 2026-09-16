using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.DTOs.Schemas;
using System.Net.Http.Json;
using System.Net;

namespace GraphForge.Api.IntegrationTests.SchemasControllerTests;

public sealed class UpdateSchemaTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public UpdateSchemaTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task UpdateSchema_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project, SchemaResponse schema) = await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();

        HttpClient client = _apiTestFactory.CreateClient();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/schemas/{schema.Id}", new
        {
            schemaTypeName = "ValidName",
            fields = new List<SchemaFieldUpdateRequest>()
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task UpdateSchema_WhenSchemaDoesNotExist_Returns404ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/schemas/{Guid.NewGuid()}", new
        {
            schemaTypeName = "Name",
            fields = new List<SchemaFieldUpdateRequest>()
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task UpdateSchema_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{Guid.NewGuid()}/schemas/{Guid.NewGuid()}", new
        {
            schemaTypeName = "ValidName",
            fields = new List<SchemaFieldUpdateRequest>()
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task UpdateSchema_WhenProjectBelongsToOtherUser_Returns404ProblemDetails()
    {
        var createdFields = RandomFieldsFactory.GetFieldsRandomValidData();

        (HttpClient ownerClient, ProjectInfoResponse project, SchemaResponse createdSchema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync(fields: createdFields);

        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/schemas/{createdSchema.Id}",
            new
            {
                schemaTypeName = "ValidName",
                fields = new List<SchemaFieldUpdateRequest>()
            }
            );


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);

        HttpResponseMessage getResponse = await ownerClient.GetAsync($"/api/projects/{project.Id}/schemas/{createdSchema.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        SchemaResponse? actualSchema = await getResponse.Content.ReadFromJsonAsync<SchemaResponse>();
        Assert.NotNull(actualSchema);

        Assert.Equal(createdSchema.SchemaTypeName, actualSchema.SchemaTypeName);
        AssertSchemaFieldsEqual(createdFields, actualSchema.Fields);
    }


    [Fact]
    public async Task UpdateSchema_WhenDataCorrect_Returns200WithNewData()
    {
        string newSchemaTypeName = "AbsolutelyUniqueFunnyNameOfSchema";

        (HttpClient client, ProjectInfoResponse project, SchemaResponse createdSchema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync(fields: RandomFieldsFactory.GetFieldsRandomValidData());

        List<SchemaFieldUpdateRequest> newFields = createdSchema.Fields.Select((field) =>
            new SchemaFieldUpdateRequest(field.Id, field.Name + "New", RandomFieldsFactory.GetRandomType())
            ).ToList();
        newFields.RemoveAt(0);
        newFields.Add(new SchemaFieldUpdateRequest(null, "newField", "string"));
        

        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/schemas/{createdSchema.Id}",
            new
            {
                schemaTypeName = newSchemaTypeName,
                fields = newFields
            }
            );


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        SchemaResponse? actualSchema = await response.Content.ReadFromJsonAsync<SchemaResponse>();
        Assert.NotNull(actualSchema);

        Assert.Equal(newSchemaTypeName, actualSchema.SchemaTypeName);
        Assert.Equal(createdSchema.Id, actualSchema.Id);
        AssertSchemaFieldsEqual(newFields, actualSchema.Fields);
    }


    [Fact]
    public async Task UpdateSchema_WhenNameIsMissing_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse createdSchema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/schemas/{createdSchema.Id}",
            new
            {
                fields = new List<SchemaFieldUpdateRequest>()
            }
            );

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateSchema_WhenFieldsAreNull_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse createdSchema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/schemas/{createdSchema.Id}",
            new
            {
                schemaTypeName = "Name",
                fields = (List<SchemaFieldUpdateRequest>?)null
            }
            );

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateSchema_WhenFieldsAreMissing_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse createdSchema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/schemas/{createdSchema.Id}",
            new
            {
                schemaTypeName="Name"                
            }
            );

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateSchema_WhenSchemaWithSameNameExists_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project, SchemaResponse firstSchema) =
            await _apiTestFactory.CreateAuthorizedClientWithSchemaAsync(schemaTypeName: "FirstSchema");

        HttpResponseMessage createResponse = await client.PostAsJsonAsync($"/api/projects/{project.Id}/schemas", new
        {
            schemaTypeName = "SecondSchema",
            fields = new List<SchemaFieldCreationRequest>()
        });

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        SchemaResponse? secondSchema = await createResponse.Content.ReadFromJsonAsync<SchemaResponse>();
        Assert.NotNull(secondSchema);


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{project.Id}/schemas/{secondSchema.Id}",
            new
            {
                schemaTypeName = firstSchema.SchemaTypeName,
                fields = new List<SchemaFieldUpdateRequest>()
            }
            );

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }
}
