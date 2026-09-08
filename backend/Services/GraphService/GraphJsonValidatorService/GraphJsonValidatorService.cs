using GraphForge.Contracts;
using System.Text.Json;

namespace GraphForge.Api.Services.GraphService.GraphJsonValidatorService;

public class GraphJsonValidatorService : IGraphJsonValidatorService
{
    public void Validate(JsonDocument graphJson)
    {
        try
        {
            GraphDto? graph = JsonSerializer.Deserialize<GraphDto>(graphJson);
            if (graph == null)
            {
                throw new GraphValidationException("Failed to deserialize json: null");
            }
            ValidateDto(graph);
        }
        catch (JsonException ex) {
            throw new GraphValidationException($"Error during parsing json: {ex.Message}");
        }
    }

    private void ValidateDto(GraphDto graph)
    {
        if (graph.Nodes is null)
        {
            throw new GraphValidationException("Graph nodes are required.");
        }

        if (graph.Edges is null)
        {
            throw new GraphValidationException("Graph edges are required.");
        }

        var nodeIds = graph.Nodes
            .Select(n => n.Id)
            .ToHashSet();

        foreach (var edge in graph.Edges)
        {
            if (!nodeIds.Contains(edge.Source))
            {
                throw new GraphValidationException(
                    $"Source node '{edge.Source}' does not exist.");
            }

            if (!nodeIds.Contains(edge.Target))
            {
                throw new GraphValidationException(
                    $"Target node '{edge.Target}' does not exist.");
            }
        }
    }
}
