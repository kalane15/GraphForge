using System.Text.Json;

namespace GraphForge.Api.DTOs.Schemas;

public record SchemaResponse(Guid Id, string SchemaTypeName, List<SchemaFieldDefinitionResponse> Fields);
