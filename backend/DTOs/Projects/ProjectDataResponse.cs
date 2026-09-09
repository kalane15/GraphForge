using GraphForge.Api.DTOs.Graphs;

namespace GraphForge.Api.DTOs.Projects;

public sealed record ProjectDataResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<GraphInfoResponse> Graphs
);
