namespace GraphForge.Api.DTOs.Schemas;

public sealed record SchemaFieldDefinitionResponse(
    Guid Id,
    string Name,
    string Type
);
