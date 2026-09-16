using GraphForge.Contracts;

namespace GraphForge.Api.DTOs.Graphs;

public record GraphDataResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    GraphDto Content
);
