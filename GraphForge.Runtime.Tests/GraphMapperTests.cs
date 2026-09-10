using GraphForge.Contracts;
using GraphForge.Generated;
using System.Text.Json;

namespace GraphForge.Runtime.Tests;

public sealed class GraphMapperTests
{
    [Fact]
    public void ToDtoFromDto_WhenGraphIsMappedToDtoAndBack_PreservesMetadata()
    {
        Guid schemaId = Guid.NewGuid();

        var dto = new GraphDto
        {
            Nodes = new List<NodeDto>
            {
                new NodeDto
                {
                    Id = "node-1",
                    Position = new PositionDto
                    {
                        X = 4,
                        Y = 5
                    },
                    ReactFlowType = "someReactFlowType",
                    Data = new NodeDataDto
                    {
                        Title = "title",
                        SchemaId = schemaId,
                        SchemaTypeName = "DialogueNode",
                        Properties = JsonSerializer.SerializeToElement(new
                        {
                            speaker = "Alice"
                        })
                    }
                },
                new NodeDto
                {
                    Id = "node-2",
                    Position = new PositionDto
                    {
                        X = 6,
                        Y = 7
                    },
                    ReactFlowType = "editableNode",
                    Data = new NodeDataDto
                    {
                        Title = "reply",
                        SchemaId = schemaId,
                        SchemaTypeName = "DialogueNode",
                        Properties = JsonSerializer.SerializeToElement(new
                        {
                            speaker = "Bob"
                        })
                    }
                }
            },
            Edges = new List<EdgeDto>
            {
                new EdgeDto
                {
                    Id = "edge-1",
                    Source = "node-1",
                    Target = "node-2",
                    SourceHandle = "right",
                    TargetHandle = "left"
                }
            }
        };

        Graph graph = GraphMapper.FromDto(dto);
        GraphDto mappedDto = GraphMapper.ToDto(graph);

        //Mapper does not preserves order, so we need this to check values of certain nodes
        NodeDto node1 = mappedDto.Nodes.First((node) => node.Id == "node-1");
        NodeDto node2 = mappedDto.Nodes.First((node) => node.Id == "node-2");
        EdgeDto edge = mappedDto.Edges[0];

        Assert.Equal(schemaId, node1.Data.SchemaId);
        Assert.Equal("DialogueNode", node1.Data.SchemaTypeName);

        Assert.Equal(4, node1.Position.X);
        Assert.Equal(5, node1.Position.Y);

        Assert.Equal("someReactFlowType", node1.ReactFlowType);

        Assert.Equal(2, mappedDto.Nodes.Count);

        Assert.Equal("node-2", node2.Id);

        DialogueNode? dialogueNode = node1.Data.Properties.Deserialize<DialogueNode>();
        Assert.NotNull(dialogueNode);
        Assert.Equal("Alice", dialogueNode.Speaker);

        Assert.False(string.IsNullOrWhiteSpace(edge.Id));
        
        Assert.Equal("edge-1", edge.Id);

        Assert.Equal("node-1", edge.Source);
        Assert.Equal("node-2", edge.Target);

        Assert.Equal("right", edge.SourceHandle);
        Assert.Equal("left", edge.TargetHandle);
    }
}
