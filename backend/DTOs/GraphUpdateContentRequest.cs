using System.Text.Json;

namespace GraphForge.Api.DTOs;

public sealed record UpdateGraphContentRequest(JsonDocument Content);
