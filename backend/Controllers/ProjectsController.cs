using GraphForge.Api.Auth;
using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GraphForge.Api.Controllers;


[Route("api/projects")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectsList()
    {
        string? login = User.Identity?.Name;

        if (login == null)
        {
            return StatusCode(500, new
            {
                message = "User claim does not exist"
            });
        }

        

        return Ok(new ProjectsListResponse(Array.Empty<ProjectInfo>()));
    }
}
