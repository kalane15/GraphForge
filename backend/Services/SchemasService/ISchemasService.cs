using GraphForge.Api.DTOs;

namespace GraphForge.Api.Services.SchemasService;

public interface ISchemasService
{
    Task<SchemasListResponse> GetSchemasList(Guid userId, Guid projectId);
}
