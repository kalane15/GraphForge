using System.Net;
using System.Net.Http.Json;
using GraphForge.Api.DTOs.Projects;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;

public sealed class GetProjectTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    public GetProjectTests(ApiPostgresTestFactory factory)
    {
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
        (HttpClient client, ProjectInfoResponse info) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync(name, description);

        HttpResponseMessage getResponse = await client.GetAsync($"/api/projects/{info.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        ProjectDataResponse? projectData = await getResponse.Content.ReadFromJsonAsync<ProjectDataResponse>();
        Assert.NotNull(projectData);

        Assert.Equal(name, projectData.Name);
        Assert.Equal(description, projectData.Description);
        Assert.Equal(info.Id, projectData.Id);
    }

    [Theory]
    [MemberData(nameof(ProjectRandomData))]
    public async Task GetProject_WhenProjectBelongsToOtherUser_Returns404ProblemDetail(string name, string description)
    {
        (_, ProjectInfoResponse info) =
            await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync(name, description);

        await AssertAccessOtherUserResourceReturnsNotFoundAsync(
            _apiTestFactory,
            client => client.GetAsync($"/api/projects/{info.Id}"));
    }
}
