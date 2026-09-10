using GraphForge.Contracts;
using System.Text.Json;

namespace GraphForge.Validation.Tests;

public sealed class GraphDtoBuilder
{
    private readonly GraphDto _graph = new();

    public GraphDtoBuilder WithNode(string id, Guid schemaId)
    {
        _graph.Nodes.Add(new NodeDto
        {
            Id = id,
            Data = CreateNodeData(schemaId)
        });

        return this;
    }

    public GraphDtoBuilder WithNode(Guid schemaId)
    {
        _graph.Nodes.Add(new NodeDto
        {
            Id = Guid.NewGuid().ToString(),
            Data = CreateNodeData(schemaId)
        });

        return this;
    }

    public GraphDtoBuilder WithNode(NodeDto node)
    {
        _graph.Nodes.Add(node);

        return this;
    }

    public GraphDtoBuilder WithEdge(
        string id,
        string source = "node-1",
        string target = "node-2")
    {
        _graph.Edges.Add(new EdgeDto
        {
            Id = id,
            Source = source,
            Target = target,
            SourceHandle = "right",
            TargetHandle = "left"
        });

        return this;
    }

    public GraphDtoBuilder WithEdge(
        string source = "node-1",
        string target = "node-2")
    {
        _graph.Edges.Add(new EdgeDto
        {
            Id = Guid.NewGuid().ToString(),
            Source = source,
            Target = target,
            SourceHandle = "right",
            TargetHandle = "left"
        });

        return this;
    }

    public GraphDto Build() => _graph;

    private static NodeDataDto CreateNodeData(Guid schemaId)
    {
        return new NodeDataDto
        {
            Title = "Node",
            SchemaId = schemaId,
            SchemaTypeName = "DialogueNode",
            Properties = JsonSerializer.SerializeToElement(new Dictionary<string, object?>())
        };
    }
}
