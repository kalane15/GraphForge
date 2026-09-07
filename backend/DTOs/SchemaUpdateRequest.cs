using System.Text.Json;

namespace GraphForge.Api.DTOs;

public sealed record SchemaUpdateRequest(
    string SchemaTypeName,
    JsonDocument Content
);
