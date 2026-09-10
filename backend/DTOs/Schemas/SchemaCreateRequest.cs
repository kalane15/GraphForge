using System.Text.Json;

namespace GraphForge.Api.DTOs.Schemas;

public sealed record SchemaCreateRequest(string SchemaTypeName, List<SchemaFieldCreationRequest> Fields);
