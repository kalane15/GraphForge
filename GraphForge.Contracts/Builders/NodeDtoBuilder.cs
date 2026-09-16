using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GraphForge.Contracts;

public sealed class NodeDtoBuilder
{
    private readonly Dictionary<string, object?> _properties = new Dictionary<string, object?>();

    private readonly NodeDto _node = new NodeDto
    {
        Id = Guid.NewGuid().ToString(),
        Data = new NodeDataDto
        {
            Title = "Node",
            SchemaTypeName = "DialogueNode"
        }
    };

    public NodeDtoBuilder WithId(string id)
    {
        _node.Id = id;
        return this;
    }

    public NodeDtoBuilder WithSchemaId(Guid schemaId)
    {
        _node.Data.SchemaId = schemaId;
        return this;
    }

    public NodeDtoBuilder WithSchemaTypeName(string schemaTypeName)
    {
        _node.Data.SchemaTypeName = schemaTypeName;
        return this;
    }

    public NodeDtoBuilder WithProperty(string name, object? value)
    {
        _properties[name] = value;
        return this;
    }

    public NodeDto Build()
    {
        _node.Data.Properties = JsonSerializer.SerializeToElement(_properties);
        return _node;
    }
}
