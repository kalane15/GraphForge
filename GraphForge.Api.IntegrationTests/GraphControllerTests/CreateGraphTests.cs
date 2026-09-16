using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.GraphControllerTests;

public sealed class CreateGraphTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public CreateGraphTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task CreateGraph_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse project) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpClient client = _apiTestFactory.CreateClient();


        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{project.Id}/graphs", new
        {
            name="name"
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task CreateGraph_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{Guid.NewGuid()}/graphs", new
        {
            name = "name"
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task CreateGraph_WhenProjectBelongsToOtherUser_Returns404ProblemDetails()
    {
        (_, ProjectInfoResponse project) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{project.Id}/graphs", new
        {
            name = "name"
        });


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task CreateGraph_WhenNameIsMissing_Returns400ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse project) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{project.Id}/graphs", new {});


        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }



    [Fact]
    public async Task CreateGraph_WhenDataCorrect_Returns200WithCreatedData()
    {
        string name = "graph name";
        (HttpClient client, ProjectInfoResponse project) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects/{project.Id}/graphs", new 
        {
            name
        });


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GraphInfoResponse? info = await response.Content.ReadFromJsonAsync<GraphInfoResponse>();
        Assert.NotNull(info);

        Assert.Equal(name, info.Name);
        Assert.Equal(project.Id, info.ProjectId);

        Assert.NotEqual(Guid.Empty, info.Id);
        Assert.NotEqual(default, info.CreatedAt);
        Assert.NotEqual(default, info.UpdatedAt);
    }
}
