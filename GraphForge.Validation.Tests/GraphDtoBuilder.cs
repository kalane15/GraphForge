using GraphForge.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.Tests;

public sealed class GraphDtoBuilder
{
    private readonly GraphDto _graph = new();

    public GraphDtoBuilder WithNode(string id, Guid schemaId)
    {
        _graph.Nodes.Add(new NodeDto
        {
            Id = id,
            Data = new NodeDataDto
            {
                SchemaId = schemaId,
                SchemaTypeName = "DialogueNode"
            }
        });

        return this;
    }

    public GraphDtoBuilder WithNode(Guid schemaId)
    {
        _graph.Nodes.Add(new NodeDto
        {
            Id = Guid.NewGuid().ToString(),
            Data = new NodeDataDto
            {
                SchemaId = schemaId,
                SchemaTypeName = "DialogueNode"
            }
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
}
