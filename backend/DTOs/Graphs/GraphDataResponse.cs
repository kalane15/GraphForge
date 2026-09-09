using System.Text.Json;

namespace GraphForge.Api.DTOs.Graphs;

public record GraphDataResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    JsonDocument Content
);
