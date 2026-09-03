using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using GraphForge.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Controllers;


[Route("api/graphs")]
[ApiController]
[Authorize]
public class GraphsController : ControllerBase
{
    private readonly AppDbContext _db;

    public GraphsController(AppDbContext db)
    {
        _db = db;
    }


    [HttpPost]
    public async Task<IActionResult> CreateGraph(CreateEmptyGraphRequest request)
    {
        bool exist = await _db.Graphs.AnyAsync(g => g.Name == request.Name && g.ProjectId == request.ProjectId);

        if (exist)
        {
            return BadRequest(new ProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Entity exists",
                    Detail = "Graph already exists"
                }
            );
        }

        var result = new Graph
        {
            Name = request.Name,
            ProjectId = request.ProjectId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Graphs.Add(result);

        await _db.SaveChangesAsync();

        return Ok(new GraphInfoResponse(
            result.Id,
            result.ProjectId,
            result.Name,
            result.CreatedAt,
            result.UpdatedAt
        ));
    }
}
