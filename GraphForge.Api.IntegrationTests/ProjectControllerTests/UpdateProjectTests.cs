using GraphForge.Api.DTOs.Projects;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;

public sealed class UpdateProjectTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public UpdateProjectTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }

    [Fact]
    public async Task UpdateProject_WhenNameEmpty_Returns400()
    {
        (HttpClient client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{info.Id}", new
        {
            name = "",
            description = "description",
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateProject_WhenNameMissing_Returns400()
    {
        (HttpClient client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{info.Id}", new
        {
            description = "description",
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task UpdateProject_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();
        

        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{Guid.NewGuid()}", new
        {
            name = "name",
            description = "description",
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProject_WhenProjectBelongsToOtherUser_Returns404ProblemDetails()
    {
        string oldName = "oldName";
        string oldDescription = "oldDescription";
        string newName = "newName";
        string newDescription = "newDescription";

        (HttpClient ownerClient, ProjectInfoResponse createdProjectInfo) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync(oldName, oldDescription);


        HttpClient otherClient = await _apiTestFactory.CreateAuthorizedClientAsync();
        HttpResponseMessage response = await otherClient.PutAsJsonAsync($"/api/projects/{createdProjectInfo.Id}", new
        {
            name = newName,
            description = newDescription
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);

        response = await ownerClient.GetAsync($"/api/projects/{createdProjectInfo.Id}");
        ProjectInfoResponse? getInfo = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();

        Assert.NotNull(createdProjectInfo);
        Assert.Equal(oldName, createdProjectInfo.Name);
        Assert.Equal(oldDescription, createdProjectInfo.Description);
    }


    [Fact]
    public async Task UpdateProject_WhenUnauthorized_Returns401ProblemDetails()
    {
        (var _, ProjectInfoResponse info) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpClient client = _apiTestFactory.CreateClient();


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{info.Id}", new
        {
            name = "name",
            description = "description",
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task UpdateProject_WhenDataCorrect_Returns200WithNewData()
    {
        string oldName = "oldName";
        string oldDescription = "oldDescription";
        string newName = "newName";
        string newDescription = "newDescription";

        (HttpClient client, ProjectInfoResponse createdProjectInfo) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync(oldName, oldDescription); ;


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{createdProjectInfo.Id}", new
        {
            name = newName,
            description = newDescription,
        });


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        Assert.Equal(newName, info.Name);
        Assert.Equal(newDescription, info.Description);
        Assert.Equal(createdProjectInfo.Id, info.Id);
    }


    /// <summary>
    /// Returns 200 because description is optional
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateProject_WhenDesciptionMissing_Returns200WithNewName()
    {
        string oldName = "oldName";
        string newName = "newName";

        (HttpClient client, ProjectInfoResponse createdProjectInfo) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync(oldName); ;


        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/projects/{createdProjectInfo.Id}", new
        {
            name = newName
        });


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        Assert.Equal(newName, info.Name);
        Assert.Equal(createdProjectInfo.Id, info.Id);
    }
}
