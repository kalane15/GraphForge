namespace GraphForge.Api.DTOs;

public sealed record SchemaDefinition(
    string SchemaTypeName,
    IReadOnlyList<SchemaFieldDefinition> Fields
);

public sealed record SchemaFieldDefinition(
    string Name,
    string Type
);
