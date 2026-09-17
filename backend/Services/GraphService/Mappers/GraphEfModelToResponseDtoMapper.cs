using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.Models;
using GraphForge.Contracts;

namespace GraphForge.Api.Services.GraphService.Mappers;

internal static class GraphEfModelMapper
{
    public static GraphInfoResponse ToInfoResponse(Graph graph)
    {
        return new GraphInfoResponse(
            graph.Id,
            graph.ProjectId,
            graph.Name,
            graph.CreatedAt,
            graph.UpdatedAt
        );
    }

    public static GraphDataResponse ToDataResponse(Graph graph)
    {
        return ToDataResponse(
            graph,
            GraphContentMapper.ToDto(graph.Content)
        );
    }

    public static GraphDataResponse ToDataResponse(Graph graph, GraphDto content)
    {
        return new GraphDataResponse(
            graph.Id,
            graph.ProjectId,
            graph.Name,
            content
        );
    }
}
