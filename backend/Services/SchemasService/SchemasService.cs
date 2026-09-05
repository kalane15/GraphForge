using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Services.GraphService;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Services.SchemasService;

public class SchemasService : ISchemasService
{
    private readonly AppDbContext _db;


    public SchemasService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SchemasListResponse> GetSchemasList(Guid userId, Guid projectId)
    {
        bool isProjectBelongsToUser = await _db.Projects.AnyAsync((p) => p.Id == projectId && p.OwnerId == userId);

        if (!isProjectBelongsToUser)
        {
            throw new IncorrectProjectOwnerException("Project does not belong to the user");
        }

        SchemasListResponse res = new SchemasListResponse
            (
                await _db.Schemas
                    .Where((schema) => schema.ProjectId == projectId)
                      .Select((s) => new SchemaResponse(s.Id, s.SchemaTypeName, s.Content))
                        .ToListAsync()
            );

        return res;
    }
}
