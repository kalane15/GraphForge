using GraphForge.Contracts;

namespace GraphForge.Api.IntegrationTests;

internal static class GraphDtoFactory
{
    public static GraphDto CreateValidGraph(Guid schemaId)
    {
        string firstNodeId = Guid.NewGuid().ToString();
        string secondNodeId = Guid.NewGuid().ToString();

        return new GraphDtoBuilder()
            .WithNode(firstNodeId, schemaId)
            .WithNode(secondNodeId, schemaId)
            .WithEdge(source: firstNodeId, target: secondNodeId)
            .Build();
    }
}
