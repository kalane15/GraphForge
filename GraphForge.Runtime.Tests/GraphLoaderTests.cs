using GraphForge.Runtime;

namespace GraphForge.Runtime.Tests;

public sealed class GraphLoaderTests
{
    [Fact]
    public void LoadGraphFromFile_LoadsDialogueGraph()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "dialogue-graph.json");

        Graph graph = GraphLoader.LoadGraphFromFile(path);

        Assert.Equal(2, graph.Nodes.Count);
        Assert.Single(graph.Edges);
        Assert.Equal("Start", graph.Nodes[0].Title);
        Assert.Equal("Reply", graph.Nodes[1].Title);
        Assert.Same(graph.Nodes[0], graph.Edges[0].SourceNode);
        Assert.Same(graph.Nodes[1], graph.Edges[0].TargetNode);
    }

    [Fact]
    public void LoadGraphAndSavePreservesTitleChangesInRuntime()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "dialogue-graph.json");

        Graph graph = GraphLoader.LoadGraphFromFile(path);
        graph.Nodes[0].Title = "new title";
        path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "dialogue-graph-saved.json");
        GraphLoader.SaveGraphToFile(path, graph);

        graph = GraphLoader.LoadGraphFromFile(path);
        Assert.Contains(graph.Nodes, node => node.Title == "new title");
        Assert.Single(graph.Edges);
    }
}
