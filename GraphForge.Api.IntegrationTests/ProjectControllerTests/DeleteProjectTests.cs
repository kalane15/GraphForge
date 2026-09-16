using GraphForge.Api.DTOs.Projects;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;
public sealed class DeleteProjectTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    public DeleteProjectTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task DeleteProject_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.DeleteAsync($"api/projects/{Guid.NewGuid()}");


        await AssertProblemDetailsAsync(response, HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task DeleteProject_WhenProjectExist_Returns204AndGetReturns404()
    {
        (HttpClient client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();


        HttpResponseMessage deleteResponse = await client.DeleteAsync($"api/projects/{info.Id}");


        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        HttpResponseMessage getResponse = await client.GetAsync($"api/projects/{info.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }


    [Fact]
    public async Task DeleteProject_WhenProjectBelongsToOtherUser_Returns404ProblemDetails()
    {
        (HttpClient client, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpClient otherclient = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage deleteResponse = await otherclient.DeleteAsync($"api/projects/{info.Id}");


        await AssertProblemDetailsAsync(deleteResponse, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProject_WhenUnauthorized_Returns401ProblemDetails()
    {
        (_, ProjectInfoResponse info) = await _apiTestFactory.CreateAuthorizedClientWithEmptyProjectAsync();

        HttpClient client = _apiTestFactory.CreateClient();


        HttpResponseMessage deleteResponse = await client.DeleteAsync($"api/projects/{info.Id}");


        await AssertProblemDetailsAsync(deleteResponse, HttpStatusCode.Unauthorized);
    }
}

