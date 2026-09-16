using GraphForge.Contracts;

namespace GraphForge.Api.IntegrationTests;

internal static class GraphDtoAssert
{
    public static void AssertGraphDtoEqual(GraphDto expected, GraphDto actual)
    {
        Assert.Equal(expected.Nodes.Count, actual.Nodes.Count);
        Assert.Equal(expected.Edges.Count, actual.Edges.Count);

        foreach (NodeDto expectedNode in expected.Nodes)
        {
            NodeDto? actualNode = actual.Nodes.FirstOrDefault(node => node.Id == expectedNode.Id);

            Assert.NotNull(actualNode);
            Assert.Equal(expectedNode.ReactFlowType, actualNode.ReactFlowType);
            Assert.Equal(expectedNode.Position.X, actualNode.Position.X);
            Assert.Equal(expectedNode.Position.Y, actualNode.Position.Y);
            Assert.Equal(expectedNode.Data.Title, actualNode.Data.Title);
            Assert.Equal(expectedNode.Data.SchemaId, actualNode.Data.SchemaId);
            Assert.Equal(expectedNode.Data.SchemaTypeName, actualNode.Data.SchemaTypeName);
            Assert.Equal(expectedNode.Data.Properties.GetRawText(), actualNode.Data.Properties.GetRawText());
        }

        foreach (EdgeDto expectedEdge in expected.Edges)
        {
            EdgeDto? actualEdge = actual.Edges.FirstOrDefault(edge => edge.Id == expectedEdge.Id);

            Assert.NotNull(actualEdge);
            Assert.Equal(expectedEdge.Source, actualEdge.Source);
            Assert.Equal(expectedEdge.Target, actualEdge.Target);
            Assert.Equal(expectedEdge.SourceHandle, actualEdge.SourceHandle);
            Assert.Equal(expectedEdge.TargetHandle, actualEdge.TargetHandle);
        }
    }
}
