using GraphForge.Api.DTOs.Projects;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;

public sealed class GetProjectsListTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    public GetProjectsListTests(ApiPostgresTestFactory factory) {
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

            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
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
        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.GetAsync("/api/projects"));
    }    
}
