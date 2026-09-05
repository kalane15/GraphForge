namespace GraphForge.Api.DTOs;

public sealed record ProjectDataResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<GraphInfoResponse> Graphs
);
