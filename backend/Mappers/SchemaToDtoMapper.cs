using GraphForge.Api.Models;
using GraphForge.Contracts;

namespace GraphForge.Api.Mappers;

public static class SchemaMapper
{
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

    public static SchemaFieldDto ToDto(SchemaField field)
    {
        return new SchemaFieldDto
        {
            Id = field.Id,
            Name = field.Name,
            Type = field.Type
        };
    }
}
