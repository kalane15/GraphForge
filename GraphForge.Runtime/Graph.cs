using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Runtime;

public class Graph
{
    public List<GraphNode> Nodes { get; set; } = new List<GraphNode>();
    public List<GraphEdge> Edges { get; set; } = new List<GraphEdge>();
}
