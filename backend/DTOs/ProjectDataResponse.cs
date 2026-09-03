using GraphForge.Api.Models;

namespace GraphForge.Api.DTOs
{
    public record ProjectDataResponse(
        Guid Id,
        Guid OwnerId,
        string Name,
        string? Description,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt,
        List<GraphInfoResponse> Graphs
    );
}
