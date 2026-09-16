using GraphForge.Api.DTOs.Projects;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace GraphForge.Api.IntegrationTests;

public static class SchemasTestClientExtensions
{
    /// <summary>
    /// Created authorized client and creates project 
    /// with name <paramref name="inputName"/> and description <paramref name="inputDescription"/>
    /// </summary>
    /// <returns>Returns (HttpClient, Created project DTO)</returns>
    public static async Task<(HttpClient, ProjectInfoResponse)> CreateAuthorizedClientWithEmptyProjectAsync(
        this ApiPostgresTestFactory factory, string? inputName=null, string? inputDescription=null)
    {
        var client = await factory.CreateAuthorizedClientAsync();

        string name = inputName ?? "name";
        string description = inputDescription ?? "description";

        var response = await client.PostAsJsonAsync("/api/projects", new
        {
            name,
            description
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        ProjectInfoResponse? info = await response.Content.ReadFromJsonAsync<ProjectInfoResponse>();
        Assert.NotNull(info);

        return (client, info);
    }

    public static async Task<(HttpClient, ProjectInfoResponse)> CreateUnauthorizedClientWithEmptyProjectAsync(
    this ApiPostgresTestFactory factory, string? inputName = null, string? inputDescription = null)
    {
        (_, ProjectInfoResponse info) = await factory.CreateAuthorizedClientWithEmptyProjectAsync();

        return (factory.CreateClient(), info);
    }
}
