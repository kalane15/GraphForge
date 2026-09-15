using GraphForge.Api.DTOs.Projects;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;

public sealed class GetProjectTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    public GetProjectTests(ApiPostgresTestFactory factory) {
        _apiTestFactory = factory;
    }

    public static IEnumerable<object[]> ProjectRandomData()
    {
        yield return new object[]
        {
            Guid.NewGuid().ToString("N"),
            Guid.NewGuid().ToString("N")
        };        
    }


    [Fact]
    public async Task GetProject_WhenProjectDoesNotExists_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.GetAsync($"/api/projects/{Guid.NewGuid()}");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Theory]
    [MemberData(nameof(ProjectRandomData))]
    public async Task GetProject_WhenProjectCreated_Returns200SameDataProject(string name, string description)
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects", new
        {
            name, 
            description
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);
        Guid createdProjectId = info.Id;


        HttpResponseMessage getResponse = await client.GetAsync($"/api/projects/{createdProjectId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        ProjectDataResponse? projectData = await getResponse.Content.ReadFromJsonAsync<ProjectDataResponse>();
        Assert.NotNull(projectData);

        Assert.Equal(name, projectData.Name);
        Assert.Equal(description, projectData.Description);
        Assert.Equal(createdProjectId, projectData.Id);
    }


    [Theory]
    [MemberData(nameof(ProjectRandomData))]
    public async Task GetProject_WhenProjectBelongsToOtherUser_Returns404ProblemDetail(string name, string description)
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/projects", new
        {
            name,
            description
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        Guid createdProjectId = info.Id;


        client = await _apiTestFactory.CreateAuthorizedClientAsync();

        var getResponse = await client.GetAsync($"/api/projects/{createdProjectId}");


        await AssertProblemDetailsAsync(getResponse, HttpStatusCode.NotFound);
    }
}
