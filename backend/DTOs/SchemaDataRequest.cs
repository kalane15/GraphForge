using System.Text.Json;

namespace GraphForge.Api.DTOs;

public sealed record SchemaDataRequest(
    string SchemaTypeName,
    JsonDocument Content
);
