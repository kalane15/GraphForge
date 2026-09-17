using System.Net;
using System.Net.Http.Json;
using GraphForge.Api.DTOs.Projects;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;

public class CreateProjectTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    public CreateProjectTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }

    [Fact]
    public async Task CreateProject_WhenUnauthorized_Returns401ProblemDetails()
    {
        await AssertUnauthorizedAsync(
            _apiTestFactory,
            client => client.PostAsJsonAsync("/api/projects", new
            {
                name = "name",
                description = "description"
            }));
    }

    [Fact]
    public async Task CreateProject_WhenNameIsMissing_Returns400ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/projects", new
        {
            description = "desc"
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Returns 200 because description is optional
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateProject_WhenDescriptionIsMissing_Returns200WithNewName()
    {
        string newName = "name";

        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/projects", new
        {
            name = newName
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();

        Assert.NotNull(info);
        Assert.Equal(info.Name, newName);
    }

    [Fact]
    public async Task CreateProject_WhenNameIsWhiteSpace_Returns400ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/projects", new
        {
            name = "    ",
            description = "desc"
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProject_WhenDataCorrect_Returns200()
    {
        string name = "123";
        string description = "123456";

        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/projects", new
        {
            name,
            description
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();

        Assert.NotNull(info);

        Assert.Equal(name, info.Name);
        Assert.Equal(description, info.Description);
        Assert.Equal(0, info.GraphCount);

        Assert.NotEqual(Guid.Empty, info.Id);
    }
}
