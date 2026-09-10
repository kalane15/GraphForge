using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.Services.AuthService;
using GraphForge.Api.Services.ProjectService;
using GraphForge.Api.Services.UserIdentityProviderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace GraphForge.Api.Controllers;

[Route("api/projects")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectsService _projectsService;
    private readonly IUserIdentityProvider _userIdentityProvider;


    public ProjectsController(
        IAuthService authService,
        IProjectsService projectsService,
        IUserIdentityProvider userIdentityProvider)
    {
        _projectsService = projectsService;
        _userIdentityProvider = userIdentityProvider;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(ProjectInfoEditRequest request)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();
        
        ProjectInfoResponse result = await _projectsService.CreateUserProjectAsync(userId, request);
        return Ok(result);        
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        List<ProjectInfoResponse> projects = await _projectsService.GetUserProjectsListAsync(userId);

        return Ok(new ProjectsListResponse(projects));
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        ProjectDataResponse result = await _projectsService.GetUserProjectAsync(userId, projectId);

        return Ok(result);
    }

    [HttpPut("{projectId}")]
    public async Task<IActionResult> UpdateProject(
        [FromRoute] Guid projectId,
        [FromBody] ProjectInfoEditRequest request)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();
       
        ProjectInfoResponse result = await _projectsService.UpdateUserProjectAsync(userId, projectId, request);

        return Ok(result);        
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        await _projectsService.DeleteUserProjectAsync(userId, projectId);

        return NoContent();
    }
}
