using System.Text.Json;

namespace GraphForge.Api.DTOs;

public record SchemaResponse(Guid Id, string SchemaTypeName, JsonDocument Content);
