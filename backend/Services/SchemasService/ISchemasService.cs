using GraphForge.Api.DTOs;

namespace GraphForge.Api.Services.SchemasService;

public interface ISchemasService
{
    Task<SchemasListResponse> GetSchemasList(Guid userId, Guid projectId);
    Task<SchemaResponse> CreateSchema(Guid userId, Guid projectId, SchemaCreateRequest request);
    Task UpdateSchema(Guid userId, Guid projectId, Guid schemaId, SchemaUpdateRequest request);
    Task DeleteSchema(Guid userId, Guid projectId, Guid schemaId);
}
