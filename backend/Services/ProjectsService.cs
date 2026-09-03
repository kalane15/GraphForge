using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Services;

public class ProjectsService : IProjectsService
{
    private readonly AppDbContext _db;

    public ProjectsService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<ProjectInfoResponse> CreateUserProjectAsync(Guid userId, ProjectInfoEditRequest request)
    {
        string projectName = ValidateProjectName(request);

        var newProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = projectName,
            Description = request.Description,
            OwnerId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _db.Projects.Add(newProject);
        await _db.SaveChangesAsync();

        return new ProjectInfoResponse(
                newProject.Id,
                newProject.Name,
                newProject.Description,
                0
            );
    }

    public async Task<ProjectInfoResponse[]> GetUserProjectsAsync(Guid userId)
    {
        ProjectInfoResponse[] projects = await _db.Projects
            .Where(project => project.OwnerId == userId)
            .Select(project => new ProjectInfoResponse(
                project.Id,
                project.Name,
                project.Description,
                _db.Graphs.Count(graph => graph.ProjectId == project.Id)
                )
            ).ToArrayAsync();

        return projects;
    }


    public async Task<ProjectDataResponse?> GetUserProjectAsync(Guid projectId, Guid userId)
    {
        var project = await _db.Projects
            .Include(p => p.Graphs)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

        if (project == null)
        {
            return null;
        }

        var result = new ProjectDataResponse(
            project.Id,
            project.OwnerId,
            project.Name,
            project.Description,
            project.CreatedAt,
            project.UpdatedAt,
            project.Graphs.Select(g => new GraphInfoResponse(g.Id, g.ProjectId, g.Name, g.CreatedAt, g.UpdatedAt)).ToList()
        );

        return result;
    }

    public async Task<ProjectInfoResponse?> UpdateUserProjectAsync(Guid projectId, Guid userId, ProjectInfoEditRequest request)
    {
        string projectName = ValidateProjectName(request);

        var project = await _db.Projects
           .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

        if (project == null)
        {
            return null;
        }

        var updatedProject = new Project
        {
            Id = project.Id,
            OwnerId = project.OwnerId,
            Name = projectName,
            Description = request.Description,
            CreatedAt = project.CreatedAt,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        project.Name = projectName;
        project.Description = request.Description;
        project.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();

        int graphCount = await _db.Graphs.CountAsync(graph => graph.ProjectId == project.Id);

        var result = new ProjectInfoResponse(
            project.Id,
            project.Name,
            project.Description,
            graphCount
        );

        return result;
    }

    public async Task<bool> DeleteUserProjectAsync(Guid projectId, Guid userId)
    {
        var project = await _db.Projects
           .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == userId);

        if (project == null)
        {
            return false;
        }

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }

    private static string ValidateProjectName(ProjectInfoEditRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ProjectValidationException("Project name is required");
        }

        return request.Name.Trim();
    }
}
