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
    public string Id = string.Empty;
    public string SourceHandle = "left";
    public string TargetHandle = "right";

    public GraphEdgeMetadata(string id, string sourceHandle, string targetHandle)
    {
        Id = id;
        SourceHandle = sourceHandle;
        TargetHandle = targetHandle;
    }

    public GraphEdgeMetadata()
    {
    }
}
