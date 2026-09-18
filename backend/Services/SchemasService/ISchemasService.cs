using GraphForge.Api.DTOs.Schemas;

namespace GraphForge.Api.Services.SchemasService;

public interface ISchemasService
{
    Task<SchemasListResponse> GetSchemasListAsync(Guid userId, Guid projectId);
    Task<SchemaResponse> GetSchemaAsync(Guid userId, Guid projectId, Guid schemaId);
    Task<SchemaResponse> CreateSchemaAsync(Guid userId, Guid projectId, SchemaCreateRequest request);
    Task<SchemaResponse> UpdateSchemaAsync(Guid userId, Guid projectId, Guid schemaId, SchemaEditDataRequest request);
    Task DeleteSchemaAsync(Guid userId, Guid projectId, Guid schemaId);
}
