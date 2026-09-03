using GraphForge.Api.DTOs;
using GraphForge.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphForge.Api.Controllers;

[Route("api/projects")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProjectsService _projectsService;

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
            return BadRequest(new { message = exception.Message });
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
            return NotFound();
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
                return NotFound();
            }

            return Ok(result);
        }
        catch (ProjectValidationException exception)
        {
            return BadRequest(new { message = exception.Message });
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

        return status ? NoContent() : NotFound();
    }

    private IActionResult? TryGetUserId(out Guid userId)
    {
        Guid? currentUserId = _authService.GetCurrentUserId();

        if (currentUserId is null)
        {
            userId = Guid.Empty;
            return StatusCode(500, new { message = "Unable to get user id" });
        }

        userId = currentUserId.Value;
        return null;
    }
}
