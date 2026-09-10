using GraphForge.Api.Database;
using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.Models;
using GraphForge.Api.Services;
using GraphForge.Api.Services.GraphService;
using GraphForge.Api.Services.ProjectService;
using GraphForge.Api.Services.UserIdentityProviderService;
using GraphForge.Validation.GraphValidationService;
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

    public GraphsController(IUserIdentityProvider userIdentityProvider, IGraphsService graphsService)
    {
        _userIdentityProvider = userIdentityProvider;
        _graphsService = graphsService;
    }


    [HttpPost]
    public async Task<IActionResult> CreateGraph(Guid projectId, GraphCreationRequest request)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();
        
        GraphInfoResponse result = await _graphsService.CreateUserGraphAsync(userId, projectId, request);
        return Ok(result);             
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

        GraphDataResponse result = await _graphsService.GetUserGraphAsync(userId, projectId, graphId);

        return Ok(result);
    }

    [HttpPut("{graphId}")]
    public async Task<IActionResult> UpdateGraph(
        [FromRoute] Guid projectId,
        [FromRoute] Guid graphId,
        [FromBody] GraphDataEditRequest request)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();
        
        GraphDataResponse? result = await _graphsService.UpdateUserGraphAsync(userId, projectId, graphId, request);
        return Ok(result);       
    }

    [HttpDelete("{graphId}")]
    public async Task<IActionResult> DeleteGraph(Guid projectId, Guid graphId)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        await _graphsService.DeleteUserGraphAsync(userId, projectId, graphId);

        return NoContent();
    }

    [HttpPut("{graphId}/content")]
    public async Task<IActionResult> UpdateUserGraphContent(
        Guid projectId,
        Guid graphId,
        UpdateGraphContentRequest request)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();

        
        await _graphsService.UpdateUserGraphContentAsync
            (
            userId,
            projectId,
            graphId,
            request.Content
            );       

        return NoContent();
    }
}
