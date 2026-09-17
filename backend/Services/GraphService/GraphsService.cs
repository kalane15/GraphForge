using GraphForge.Api.Database;
using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.Models;
using GraphForge.Api.Services.GraphService.Mappers;
using GraphForge.Api.Services.SchemasService.Mappers;
using GraphForge.Contracts;
using GraphForge.Validation.GraphValidationService;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Services.GraphService;

public class GraphsService : IGraphsService
{
    private readonly AppDbContext _db;
    private readonly IGraphJsonValidatorService _graphJsonValidatorService;

    public GraphsService(AppDbContext db, IGraphJsonValidatorService graphJsonValidatorService)
    {
        _db = db;
        _graphJsonValidatorService = graphJsonValidatorService;
    }

    public async Task<GraphInfoResponse> CreateUserGraphAsync(Guid userId, Guid projectId, GraphCreationRequest request)
    {
        string graphName = ValidateGraphName(request.Name);
        await EnsureProjectBelongsToUser(userId, projectId);

        var now = DateTimeOffset.UtcNow;
        var newGraph = new Graph
        {
            Name = graphName,
            ProjectId = projectId,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Graphs.Add(newGraph);
        await _db.SaveChangesAsync();

        return GraphMapper.ToInfoResponse(newGraph);
    }

    public async Task DeleteUserGraphAsync(Guid userId, Guid projectId, Guid graphId)
    {
        Graph graph = await GetUserGraphOrThrowAsync(userId, projectId, graphId);

        _db.Graphs.Remove(graph);
        await _db.SaveChangesAsync();
    }

    public async Task<GraphDataResponse> GetUserGraphAsync(Guid userId, Guid projectId, Guid graphId)
    {
        Graph graph = await GetUserGraphOrThrowAsync(userId, projectId, graphId);

        return GraphMapper.ToDataResponse(graph);
    }

    public async Task<List<GraphInfoResponse>> GetUserProjectsGraphsAsync(Guid userId, Guid projectId)
    {
        await EnsureProjectBelongsToUser(userId, projectId);

        List<Graph> graphs = await _db.Graphs.Where(
            (g) => g.ProjectId == projectId && g.Project.OwnerId == userId)
            .ToListAsync();

        return graphs
            .Select(GraphMapper.ToInfoResponse)
            .ToList();
    }

    public async Task<GraphDataResponse> UpdateUserGraphAsync(Guid userId, Guid projectId, Guid graphId, GraphDataEditRequest request)
    {
        string graphName = ValidateGraphName(request.Name);
        Graph graph = await GetUserGraphOrThrowAsync(userId, projectId, graphId);

        List<SchemaDto> schemas = await LoadProjectSchemas(projectId);
        _graphJsonValidatorService.Validate(request.Content, schemas);

        graph.Name = graphName;
        graph.Content = GraphContentMapper.ToJsonDocument(request.Content);
        graph.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();

        return GraphMapper.ToDataResponse(graph, request.Content);
    }

    public async Task UpdateUserGraphContentAsync(Guid userId, Guid projectId, Guid graphId, GraphForge.Contracts.GraphDto content)
    {
        Graph graph = await GetUserGraphOrThrowAsync(userId, projectId, graphId);

        List<SchemaDto> schemas = await LoadProjectSchemas(projectId);
        _graphJsonValidatorService.Validate(content, schemas);

        graph.Content = GraphContentMapper.ToJsonDocument(content);
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

    private async Task<Graph> GetUserGraphOrThrowAsync(Guid userId, Guid projectId, Guid graphId)
    {
        Graph? graph = await _db.Graphs.FirstOrDefaultAsync(
            graph =>
                graph.Id == graphId &&
                graph.ProjectId == projectId &&
                graph.Project.OwnerId == userId
        );

        if (graph is null)
        {
            throw new NotFoundException("Graph not found");
        }

        return graph;
    }

    private async Task EnsureProjectBelongsToUser(Guid userId, Guid projectId)
    {
        bool isProjectBelongsToUser = await _db.Projects.AnyAsync(project =>
            project.Id == projectId &&
            project.OwnerId == userId
        );

        if (!isProjectBelongsToUser)
        {
            throw new IncorrectProjectOwnerException("Project does not belong to the user");
        }
    }

    private async Task<List<SchemaDto>> LoadProjectSchemas(Guid projectId)
    {
        List<Schema> schemas = await _db.Schemas
            .Where(schema => schema.ProjectId == projectId)
            .Include(schema => schema.Fields)
            .ToListAsync();

        List<SchemaDto> schemaDtos = schemas
            .Select(SchemaMapper.ToDto)
            .ToList();

        return schemaDtos;
    }
}
