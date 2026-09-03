using GraphForge.Api.DTOs;
using GraphForge.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace GraphForge.Api.Controllers;

[Route("api/projects")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProjectsService _projectsService;
    private static ProblemDetails ProjectDoesNotExistsDetails() => new ()
    {
        Status = StatusCodes.Status404NotFound,
        Title = "Not found",
        Detail = "Project does not exist"
    };

    public ProjectsController(
        IAuthService authService,
        IProjectsService projectsService)
    {
        _authService = authService;
        _projectsService = projectsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(ProjectInfoEditRequest request)
    {
        IActionResult? error = TryGetUserId(out Guid userId);
        if (error is not null)
        {
            return error;
        }

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
        IActionResult? error = TryGetUserId(out Guid userId);
        if (error is not null)
        {
            return error;
        }

        ProjectInfoResponse[] projects = await _projectsService.GetUserProjectsAsync(userId);

        return Ok(new ProjectsListResponse(projects));
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        IActionResult? error = TryGetUserId(out Guid userId);
        if (error is not null)
        {
            return error;
        }

        ProjectDataResponse? result = await _projectsService.GetUserProjectAsync(projectId, userId);

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
        IActionResult? error = TryGetUserId(out Guid userId);
        if (error is not null)
        {
            return error;
        }

        try
        {
            ProjectInfoResponse? result = await _projectsService.UpdateUserProjectAsync(projectId, userId, request);

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
        IActionResult? error = TryGetUserId(out Guid userId);
        if (error is not null)
        {
            return error;
        }

        bool status = await _projectsService.DeleteUserProjectAsync(projectId, userId);

        return status ? NoContent() : NotFound(ProjectDoesNotExistsDetails());
    }
    
    private IActionResult? TryGetUserId(out Guid userId)
    {
        Guid? currentUserId = _authService.GetCurrentUserId();

        if (currentUserId is null)
        {
            userId = Guid.Empty;
            return StatusCode(500, new ProblemDetails()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Identity error",
                    Detail = "Unable to get current user id"
                }
            );
        }

        userId = currentUserId.Value;
        return null;
    }
}
