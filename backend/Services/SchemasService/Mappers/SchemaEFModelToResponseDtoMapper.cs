using GraphForge.Api.DTOs.Schemas;
using GraphForge.Api.Models;

namespace GraphForge.Api.Services.SchemasService.Mappers;

internal class SchemaEFModelToResponseDtoMapper
{
    public static SchemaResponse ToSchemaResponse(Schema schema)
    {
        var fields = schema.Fields
            .Select(field => new SchemaFieldDefinitionResponse(field.Id, field.Name, field.Type))
            .ToList();

        return new SchemaResponse(schema.Id, schema.SchemaTypeName, fields);
    }
}
