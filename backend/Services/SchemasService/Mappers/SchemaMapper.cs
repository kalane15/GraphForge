using GraphForge.Api.DTOs.Schemas;
using GraphForge.Api.Models;
using GraphForge.Contracts;

namespace GraphForge.Api.Services.SchemasService.Mappers;

internal static class SchemaMapper
{
    public static SchemaResponse ToResponse(Schema schema)
    {
        List<SchemaFieldDefinitionResponse> fields = schema.Fields
            .Select(field => new SchemaFieldDefinitionResponse(field.Id, field.Name, field.Type))
            .ToList();

        return new SchemaResponse(schema.Id, schema.SchemaTypeName, fields);
    }

    public static SchemaDto ToDto(Schema schema)
    {
        return new SchemaDto
        {
            Id = schema.Id,
            SchemaTypeName = schema.SchemaTypeName,
            Fields = schema.Fields
                .Select(ToDto)
                .ToList()
        };
    }

    private static SchemaFieldDto ToDto(SchemaField field)
    {
        return new SchemaFieldDto
        {
            Id = field.Id,
            Name = field.Name,
            Type = field.Type
        };
    }
}
