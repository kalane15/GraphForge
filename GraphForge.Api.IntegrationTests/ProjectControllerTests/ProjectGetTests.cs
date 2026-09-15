using GraphForge.Api.DTOs.Projects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;

public sealed class ProjectGetTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    public ProjectGetTests(ApiPostgresTestFactory factory) {
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


        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
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


    [Fact]
    public async Task GetProjectsList_WhenProjectsExist_ReturnsAllProjects()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();
        var expectedNames = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            string name = $"Project-{Guid.NewGuid():N}";
            expectedNames.Add(name);

            var createResponse = await client.PostAsJsonAsync(
                "/api/projects",
                new
                {
                    name,
                    description = $"Description {i}"
                });

            createResponse.EnsureSuccessStatusCode();
        }


        var response = await client.GetAsync("/api/projects");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectsListResponse? result = await response.Content.ReadFromJsonAsync<ProjectsListResponse>();

        Assert.NotNull(result);

        Assert.Equal(10, result.Projects.Count);

        foreach (string expectedName in expectedNames)
        {
            Assert.Contains(result.Projects, project => project.Name == expectedName);
        }
    }


    [Fact]
    public async Task GetProjectsList_WhenNoProjectsExist_ReturnsZeroProjects()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();
       

        var response = await client.GetAsync("/api/projects");


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectsListResponse? result = await response.Content.ReadFromJsonAsync<ProjectsListResponse>();

        Assert.NotNull(result);

        Assert.Empty(result.Projects);
    }


    [Fact]
    public async Task GetProjectsList_WhenUnauthorized_Returns401ProblemDetail()
    {
        HttpClient client = _apiTestFactory.CreateClient();


        var response = await client.GetAsync("/api/projects");


        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        ProblemDetails? result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(result);
        Assert.Equal(result.Status, StatusCodes.Status401Unauthorized);
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


        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        ProblemDetails? result = await getResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(result);
        Assert.Equal(result.Status, StatusCodes.Status404NotFound);
    }
}
