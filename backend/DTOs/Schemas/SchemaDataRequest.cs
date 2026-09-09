using System.Text.Json;

namespace GraphForge.Api.DTOs.Schemas;

public sealed record SchemaDataRequest(
    string SchemaTypeName,
    List<SchemaFieldUpdateRequest> Fields
);
