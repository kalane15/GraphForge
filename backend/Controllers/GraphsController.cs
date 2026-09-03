using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using GraphForge.Api.Services;
using GraphForge.Api.Services.GraphService;
using GraphForge.Api.Services.ProjectService;
using GraphForge.Api.Services.UserIdentityProviderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Controllers;


[Route("api/projects/{projectId}/graphs")]
[ApiController]
[Authorize]
public class GraphsController : ControllerBase
{
    private readonly IUserIdentityProvider _userIdentityProvider;
    private readonly IGraphsService _graphsService;
    private static ProblemDetails GraphDoesNotExistsDetails() => new()
    {
        Status = StatusCodes.Status404NotFound,
        Title = "Not found",
        Detail = "Graph does not exist"
    };


    public GraphsController(IUserIdentityProvider userIdentityProvider, IGraphsService graphsService)
    {
        _userIdentityProvider = userIdentityProvider;
        _graphsService = graphsService;
    }


    [HttpPost]
    public async Task<IActionResult> CreateGraph(Guid projectId, GraphCreationRequest request)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();
        
        try
        {
            GraphInfoResponse result = await _graphsService.CreateUserGraphAsync(userId, projectId, request);
            return Ok(result);
        }
        catch (GraphValidationException exception)
        {
            return BadRequest(new ProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad request",
                    Detail = exception.Message
                }
            );
        }
        catch (IncorrectProjectOwnerException exception)
        {
            return BadRequest(new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Incorrect project owner",
                Detail = exception.Message
            }
            );
        }        
    }

    [HttpGet]
    public async Task<IActionResult> GetGraphs(Guid projectId)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        List<GraphInfoResponse> graphs = await _graphsService.GetUserProjectsGraphsAsync(userId, projectId);

        return Ok(new GraphsListResponse(graphs));
    }

    [HttpGet("{graphId}")]
    public async Task<IActionResult> GetGraph(Guid projectId, Guid graphId)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        GraphDataResponse? result = await _graphsService.GetUserGraphAsync(userId, projectId, graphId);

        if (result == null)
        {
            return NotFound(GraphDoesNotExistsDetails());
        }

        return Ok(result);
    }

    [HttpPut("{graphId}")]
    public async Task<IActionResult> UpdateGraph(
        [FromRoute] Guid projectId,
        [FromRoute] Guid graphId,
        [FromBody] GraphDataEditRequest request)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        try
        {
            GraphDataResponse? result = await _graphsService.UpdateUserGraphAsync(userId, projectId, graphId, request);

            if (result == null)
            {
                return NotFound(GraphDoesNotExistsDetails());
            }

            return Ok(result);
        }
        catch (GraphValidationException exception)
        {
            return BadRequest(new ProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad request",
                    Detail = exception.Message
                }
            );
        }
        catch (IncorrectProjectOwnerException exception)
        {
            return BadRequest(new ProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Incorrect project owner",
                    Detail = exception.Message
                }
            );
        }
    }

    [HttpDelete("{graphId}")]
    public async Task<IActionResult> DeleteGraph(Guid projectId, Guid graphId)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        bool status = await _graphsService.DeleteUserGraphAsync(userId, projectId, graphId);

        return status ? NoContent() : NotFound(GraphDoesNotExistsDetails());
    }
}
