using GraphForge.Api.DTOs.Schemas;
using GraphForge.Contracts;

namespace GraphForge.Api.Services.SchemasService.Mappers;

internal static class SchemaRequestMapper
{
    public static SchemaDto ToDto(SchemaCreateRequest request)
    {
        return new SchemaDto
        {
            Id = Guid.NewGuid(),
            SchemaTypeName = request.SchemaTypeName,
            Fields = request.Fields.Select(field => new SchemaFieldDto
            {
                Id = Guid.NewGuid(),
                Name = field.Name,
                Type = field.Type
            }).ToList()
        };
    }

    public static SchemaDto ToDto(Guid schemaId, SchemaEditDataRequest request)
    {
        return new SchemaDto
        {
            Id = schemaId,
            SchemaTypeName = request.SchemaTypeName,
            Fields = request.Fields.Select(field => new SchemaFieldDto
            {
                Id = field.Id ?? Guid.NewGuid(),
                Name = field.Name,
                Type = field.Type
            }).ToList()
        };
    }
}
