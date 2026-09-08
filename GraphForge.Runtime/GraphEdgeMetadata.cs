using GraphForge.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Runtime;

/// <summary>
/// Stores metadata required for serialization and deserialization
/// but not used at runtime.
/// </summary>
internal sealed class GraphEdgeMetadata
{
    public string SourceHandle = "left";
    public string TargetHandle = "right";
    public GraphEdgeMetadata(string sourceHandle, string targetHandle)
    {
        SourceHandle = sourceHandle;
        TargetHandle = targetHandle;
    }

    public GraphEdgeMetadata()
    {
    }
}
