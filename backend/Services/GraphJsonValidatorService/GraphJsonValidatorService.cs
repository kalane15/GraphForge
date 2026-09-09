using GraphForge.Api.Services.GraphService;
using GraphForge.Contracts;

namespace GraphForge.Api.Services.GraphJsonValidatorService;

public class GraphJsonValidatorService : IGraphJsonValidatorService
{
    public void Validate(GraphDto? graph)
    {
        if (graph is null)
        {
            throw new GraphValidationException("Graph content is required.");
        }

        ValidateDto(graph);
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

        foreach (NodeDto node in graph.Nodes)
        {
            ValidateNodeData(node.Data);
        }
        
    }

    private void ValidateNodeData(NodeDataDto data)
    {
        
    }
}
