using GraphForge.Api.DTOs;
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
    private static ProblemDetails ProjectDoesNotExistsDetails() => new ()
    {
        Status = StatusCodes.Status404NotFound,
        Title = "Not found",
        Detail = "Project does not exist"
    };

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

        try
        {
            ProjectInfoResponse result = await _projectsService.CreateUserProjectAsync(userId, request);
            return Ok(result);
        }
        catch (ProjectValidationException exception)
        {
            return BadRequest(new ProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad request",
                    Detail = exception.Message
                }
            );
        }
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

        ProjectDataResponse? result = await _projectsService.GetUserProjectAsync(userId, projectId);

        if (result == null)
        {
            return NotFound(ProjectDoesNotExistsDetails());
        }

        return Ok(result);
    }

    [HttpPut("{projectId}")]
    public async Task<IActionResult> UpdateProject(
        [FromRoute] Guid projectId,
        [FromBody] ProjectInfoEditRequest request)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        try
        {
            ProjectInfoResponse? result = await _projectsService.UpdateUserProjectAsync(userId, projectId, request);

            if (result == null)
            {
                return NotFound(ProjectDoesNotExistsDetails());
            }

            return Ok(result);
        }
        catch (ProjectValidationException exception)
        {
            return BadRequest(new ProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad request",
                    Detail = exception.Message
                }
            );
        }
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        bool status = await _projectsService.DeleteUserProjectAsync(userId, projectId);

        return status ? NoContent() : NotFound(ProjectDoesNotExistsDetails());
    }
}
