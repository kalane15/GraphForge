using GraphForge.Contracts;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GraphForge.Runtime;

public static class GraphMapper
{
    private const string _generatedSourcesNamespace = "GraphForge.Generated";
    public static Graph FromDto(GraphDto dto)
    {
        var result = new Graph();

        Dictionary<string, GraphNode> IdToGraphNodeMap = new Dictionary<string, GraphNode>();

        foreach (NodeDto nodeDto in dto.Nodes)
        {
            Type? nodeType = typeof(Graph).Assembly.GetType($"{_generatedSourcesNamespace}.{nodeDto.Data.SchemaTypeName}");

            if (nodeType == null)
            {
                throw new InvalidCastException("Schema type not found in generated sources");
            }

            if (!typeof(GraphNode).IsAssignableFrom(nodeType))
            {
                throw new InvalidOperationException($"Base class of found node type is not {typeof(GraphNode).FullName}");
            }

            GraphNode node = (GraphNode)nodeDto.Data.Properties.Deserialize(nodeType)!;

            node.graphNodeMetadata = new GraphNodeMetadata(
                nodeDto.Id,
                nodeDto.ReactFlowType,
                nodeDto.Position,
                nodeDto.Data.SchemaId);
            node.Title = nodeDto.Data.Title;

            IdToGraphNodeMap[nodeDto.Id] = node;
            result.Nodes.Add(node);
        }

        foreach (EdgeDto edgeDto in dto.Edges)
        {
            GraphNode sourceNode = IdToGraphNodeMap[edgeDto.Source];
            GraphNode targetNode = IdToGraphNodeMap[edgeDto.Target];

            var edge = new GraphEdge(sourceNode, targetNode)
            {
                graphEdgeMetadata = new GraphEdgeMetadata(
                    edgeDto.Id,
                    edgeDto.SourceHandle,
                    edgeDto.TargetHandle)
            };

            result.Edges.Add(edge);
        }

        return result;
    }

    public static GraphDto ToDto(Graph graph)
    {
        var result = new GraphDto();

        Dictionary<GraphNode, string> GraphNodeToIdMap = new Dictionary<GraphNode, string>();

        foreach (GraphNode node in graph.Nodes)
        {
            JsonElement properties = JsonSerializer.SerializeToElement (
                node,
                node.GetType()
            );

            var nodeDto = new NodeDto
            {
                Id = GetOrCreateId(node.graphNodeMetadata.Id),
                ReactFlowType = node.graphNodeMetadata.ReactFlowType,
                Position = node.graphNodeMetadata.Position,
                Data = new NodeDataDto
                {
                    Title = node.Title,
                    SchemaId = node.graphNodeMetadata.SchemaId,
                    SchemaTypeName = node.GetType().Name,
                    Properties = properties
                }
            };


            GraphNodeToIdMap[node] = nodeDto.Id;

            result.Nodes.Add(nodeDto);
        }

        foreach (GraphEdge edge in graph.Edges)
        {
            var dto = new EdgeDto
            {
                Id = GetOrCreateId(edge.graphEdgeMetadata.Id),
                Source = GraphNodeToIdMap[edge.SourceNode],
                Target = GraphNodeToIdMap[edge.TargetNode],
                SourceHandle = edge.graphEdgeMetadata.SourceHandle,
                TargetHandle = edge.graphEdgeMetadata.TargetHandle,
            };

            result.Edges.Add(dto);
        }

        return result;
    }

    private static string GetOrCreateId(string id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return id;
        }

        return Guid.NewGuid().ToString();
    }
}
