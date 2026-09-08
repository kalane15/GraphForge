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
    public PositionDto Position;
    public string ReactFlowType = "editableNode";
    public GraphNodeMetadata(PositionDto pos)
    {
        Position = pos;
    }

    public GraphNodeMetadata()
    {
        Position = new PositionDto
        {
            X = 0,
            Y = 0
        };
    }
}
