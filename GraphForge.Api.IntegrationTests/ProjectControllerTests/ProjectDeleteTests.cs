using GraphForge.Api.DTOs.Projects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;
public sealed class ProjectDeleteTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;
    public ProjectDeleteTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }


    [Fact]
    public async Task DeleteProject_WhenProjectDoesNotExist_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage response = await client.DeleteAsync($"api/projects/{Guid.NewGuid()}");


        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
    }


    [Fact]
    public async Task DeleteProject_WhenProjectExist_Returns204AndGetReturns404()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"api/projects", new
        {
            name = "name",
            description = "description",
        });
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        Guid createdProjectId = info.Id;


        HttpResponseMessage deleteResponse = await client.DeleteAsync($"api/projects/{createdProjectId}");


        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        HttpResponseMessage getResponse = await client.GetAsync($"api/projects/{createdProjectId}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }


    [Fact]
    public async Task DeleteProject_WhenProjectBelongsToOtherUser_Returns404ProblemDetails()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync($"api/projects", new
        {
            name = "name",
            description = "description",
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        Guid createdProjectId = info.Id;

        HttpClient otherclient = await _apiTestFactory.CreateAuthorizedClientAsync();


        HttpResponseMessage deleteResponse = await otherclient.DeleteAsync($"api/projects/{createdProjectId}");


        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

        ProblemDetails? problem = await deleteResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
    }

    [Fact]
    public async Task DeleteProject_WhenUnauthorized_Returns401ProblemDetails()
    {
        HttpClient ownerClient = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await ownerClient.PostAsJsonAsync($"api/projects", new
        {
            name = "name",
            description = "description",
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        HttpClient client = _apiTestFactory.CreateClient();


        HttpResponseMessage deleteResponse = await client.DeleteAsync($"api/projects/{info.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);

        ProblemDetails? problem = await deleteResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status401Unauthorized, problem.Status);
    }
}

