using GraphForge.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Runtime;

/// <summary>
/// Stores metadata required for serialization and deserialization
/// but not used at runtime.
/// </summary>
internal sealed class GraphNodeMetadata
{
    public string Id = string.Empty;
    public PositionDto Position;
    public string ReactFlowType = "editableNode";
    public Guid SchemaId;

    public GraphNodeMetadata(string id, PositionDto pos, Guid schemaId)
    {
        Id = id;
        Position = pos;
        SchemaId = schemaId;
    }

    public GraphNodeMetadata()
    {
        Position = new PositionDto
        {
            X = 0,
            Y = 0
        };
        SchemaId = Guid.Empty;
    }
}
