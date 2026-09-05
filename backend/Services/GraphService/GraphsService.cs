using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using GraphForge.Api.Services.ProjectService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;


namespace GraphForge.Api.Services.GraphService
{
    public class GraphsService : IGraphsService
    {
        private readonly AppDbContext _db;


        public GraphsService(AppDbContext db)
        {
            _db = db;
        }

        async public Task<GraphInfoResponse> CreateUserGraphAsync(Guid userId, Guid projectId, GraphCreationRequest request)
        {
            string graphName = ValidateGraphName(request.Name);

            bool isProjectBelongsToUser = await _db.Projects.AnyAsync((p) => p.Id == projectId && p.OwnerId == userId);

            if (!isProjectBelongsToUser)
            {
                throw new IncorrectProjectOwnerException("Project does not belong to the user");
            }

            var newGraph = new Graph
            {
                Name = graphName,
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Graphs.Add(newGraph);
            await _db.SaveChangesAsync();

            var result = new GraphInfoResponse
            (
                newGraph.Id,
                newGraph.ProjectId,
                newGraph.Name,
                newGraph.CreatedAt,
                newGraph.UpdatedAt
            );

            return result;
        }

        public async Task DeleteUserGraphAsync(Guid userId, Guid projectId, Guid graphId)
        {
            Graph? graph = await _db.Graphs.FirstOrDefaultAsync(
                (g) => 
                    g.Id == graphId && 
                    g.ProjectId == projectId && 
                    g.Project.OwnerId == userId
            );

            if (graph == null)
            {
                throw new NotFoundException("Graph not found");
            }

            _db.Graphs.Remove(graph);
            await _db.SaveChangesAsync();
        }

        public async Task<GraphDataResponse> GetUserGraphAsync(Guid userId, Guid projectId, Guid graphId)
        {
            Graph? graph = await _db.Graphs.FirstOrDefaultAsync(
                (g) =>
                    g.Id == graphId &&
                    g.ProjectId == projectId &&
                    g.Project.OwnerId == userId
            );

            if (graph == null)
            {
                throw new NotFoundException("Graph not found");
            }

            var result = new GraphDataResponse(
                graph.Id,
                graph.ProjectId,
                graph.Name,
                graph.Content
            );

            return result;
        }

        public async Task<List<GraphInfoResponse>> GetUserProjectsGraphsAsync(Guid userId, Guid projectId)
        {
            var graphs = await _db.Graphs.Where((g) => g.ProjectId == projectId && g.Project.OwnerId == userId)
                .Select(graph => new GraphInfoResponse(
                    graph.Id,
                    graph.ProjectId,
                    graph.Name,
                    graph.CreatedAt,
                    graph.UpdatedAt)
            ).ToListAsync();

            return graphs;
        }

        public async Task<GraphDataResponse> UpdateUserGraphAsync(Guid userId, Guid projectId, Guid graphId, GraphDataEditRequest request)
        {
            string graphName = ValidateGraphName(request.Name);
            Graph? graph = await _db.Graphs.FirstOrDefaultAsync(
                (g) =>
                    g.Id == graphId &&
                    g.ProjectId == projectId &&
                    g.Project.OwnerId == userId
            );

            if (graph == null)
            {
                throw new NotFoundException("Graph not found");
            }

            graph.Name = graphName;
            graph.Content = request.Content;
            graph.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();

            var result = new GraphDataResponse(
                graph.Id,
                graph.ProjectId,
                graph.Name,
                graph.Content
            );

            return result;
        }

        public async Task UpdateUserGraphContentAsync(Guid userId, Guid projectId, Guid graphId, JsonDocument content)
        {
            Graph? graph = await _db.Graphs.FirstOrDefaultAsync(
                (g) =>
                    g.Id == graphId &&
                    g.ProjectId == projectId &&
                    g.Project.OwnerId == userId
            );

            if (graph == null)
            {
                throw new NotFoundException("Graph not found");
            }

            graph.Content = content;
            graph.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
        }

        private static string ValidateGraphName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new GraphValidationException("Graph name is required");
            }

            return name.Trim();
        }
    }
}
