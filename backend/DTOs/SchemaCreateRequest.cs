using System.Text.Json;

namespace GraphForge.Api.DTOs;

public sealed record SchemaCreateRequest(string SchemaTypeName, JsonDocument? Content);
