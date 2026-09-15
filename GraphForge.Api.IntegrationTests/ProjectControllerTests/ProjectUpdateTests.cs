using GraphForge.Api.DTOs.Projects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.ProjectControllerTests;

public sealed class ProjectUpdateTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly ApiPostgresTestFactory _apiTestFactory;

    public ProjectUpdateTests(ApiPostgresTestFactory factory)
    {
        _apiTestFactory = factory;
    }

    [Fact]
    public async Task UpdateProject_WhenNameEmpty_Returns400()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/projects", new
        {
            name = "name",
            description = "description",
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);
        Guid createdProjectId = info.Id;


        response = await client.PutAsJsonAsync($"/api/projects/{createdProjectId}", new
        {
            name = "",
            description = "description",
        });


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
    }


    [Fact]
    public async Task UpdateProject_WhenNameMissing_Returns400()
    {
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/projects", new
        {
            name = "name",
            description = "description",
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);
        Guid createdProjectId = info.Id;


        response = await client.PutAsJsonAsync($"/api/projects/{createdProjectId}", new
        {
            description = "description",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
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

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
    }

    [Fact]
    public async Task UpdateProject_WhenProjectBelongToOtherUser_Returns404ProblemDetails()
    {
        string oldName = "oldName";
        string oldDescription = "oldDescription";
        string newName = "newName";
        string newDescription = "newDescription";

        HttpClient ownerClient = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await ownerClient.PostAsJsonAsync("/api/projects", new
        {
            name = oldName,
            description = oldDescription,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);
        Guid createdProjectId = info.Id;


        HttpClient otherClient = await _apiTestFactory.CreateAuthorizedClientAsync();
        response = await otherClient.PutAsJsonAsync($"/api/projects/{createdProjectId}", new
        {
            name = newName,
            description = newDescription
        });


        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);

        response = await ownerClient.GetAsync($"/api/projects/{createdProjectId}");
        info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);
        Assert.Equal(oldName, info.Name);
        Assert.Equal(oldDescription, info.Description);
    }


    [Fact]
    public async Task UpdateProject_WhenUnauthorized_Returns401ProblemDetails()
    {
        HttpClient ownerClient = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await ownerClient.PostAsJsonAsync("/api/projects", new
        {
            name = "name",
            description = "description",
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        HttpClient client = _apiTestFactory.CreateClient();

        response = await client.PutAsJsonAsync($"/api/projects/{info.Id}", new
        {
            name = "name",
            description = "description",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal(StatusCodes.Status401Unauthorized, problem.Status);
    }


    [Fact]
    public async Task UpdateProject_WhenDataCorrect_Returns200WithNewData()
    {
        string oldName = "oldName";
        string oldDescription = "oldDescription";
        string newName = "newName";
        string newDescription = "newDescription";
        
        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/projects", new
        {
            name = oldName,
            description = oldDescription,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);
        Guid createdProjectId = info.Id;

        response = await client.PutAsJsonAsync($"/api/projects/{createdProjectId}", new
        {
            name = newName,
            description = newDescription,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        Assert.Equal(newName, info.Name);
        Assert.Equal(newDescription, info.Description);
        Assert.Equal(createdProjectId, info.Id);
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

        HttpClient client = await _apiTestFactory.CreateAuthorizedClientAsync();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/projects", new
        {
            name = oldName
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);
        Guid createdProjectId = info.Id;

        response = await client.PutAsJsonAsync($"/api/projects/{createdProjectId}", new
        {
            name = newName
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        Assert.Equal(newName, info.Name);
        Assert.Equal(createdProjectId, info.Id);
    }
}
