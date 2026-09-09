using System.Text.Json;

namespace GraphForge.Api.DTOs.Graphs;

public record GraphInfoResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
