using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Runtime;

public class GraphEdge
{
    internal GraphEdgeMetadata graphEdgeMetadata = new GraphEdgeMetadata();
    public GraphNode SourceNode;
    public GraphNode TargetNode;

    public GraphEdge (GraphNode source, GraphNode target)
    {
        SourceNode = source;
        TargetNode = target;
    }
}
