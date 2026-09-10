using GraphForge.Contracts;
using GraphForge.Validation.GraphValidationService;
using GraphForge.Validation.Tests.TestData;
using System.Text.Json;
using SchemaDtoBuilder = GraphForge.Validation.Tests.SchemaDtoBuilder;

namespace GraphForge.Validation.Tests;

public class GraphJsonValidatorServiceTests
{
    [Fact]
    public void Validate_WhenNodeIdsAreDuplicated_ThrowsDuplicateNodeIdException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("node-1", schemaId)
            .WithNode("node-1", schemaId)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<DuplicateNodeIdException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenPropertiesJsonIncorrect_ThrowsGraphPropertiesJsonInvalidException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode( 
                new NodeDto()
                {
                    Id="node",
                    Data=new NodeDataDto()
                    {
                        Title = "Node",
                        SchemaId = schemaId,
                        SchemaTypeName = "DialogueNode",
                        Properties = JsonSerializer.SerializeToElement("not an object")
                    }
                }
            )
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<GraphPropertiesJsonInvalidException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenPropertyNoName_ThrowsPropertyNameRequiredExceptionException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode(
                new NodeDtoBuilder().WithProperty("", "some value").WithSchemaId(schemaId).Build()
            )
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<PropertyNameRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenPropertiesJsonNull_ThrowsGraphPropertiesJsonInvalidException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode(
                new NodeDto()
                {
                    Id = "node",
                    Data = new NodeDataDto()
                    {
                        Title = "Node",
                        SchemaId = schemaId,
                        SchemaTypeName = "DialogueNode",
                        Properties = JsonSerializer.SerializeToElement<object?>(null)
                    }
                }
            )
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<GraphPropertiesJsonInvalidException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenEdgeIdsAreDuplicated_ThrowsDuplicateEdgeIdException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("node-1", schemaId)
            .WithNode("node-2", schemaId)
            .WithEdge(id: "edge-1")
            .WithEdge(id: "edge-1")
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<DuplicateEdgeIdException>(() => validator.Validate(graph, schemas));
    }


    [Fact]
    public void Validate_WhenNodeIdIsEmpty_ThrowsNodeIdRequiredException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("", schemaId)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<NodeIdRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenNodeTypeIsEmpty_ThrowsNodeTypeRequiredException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var node = new NodeDtoBuilder().WithSchemaId(schemaId).Build();
        node.ReactFlowType = "";

        var graph = new GraphDtoBuilder()
            .WithNode(node)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<NodeTypeRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenNodePositionIsNull_ThrowsNodePositionRequiredException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var node = new NodeDtoBuilder().WithSchemaId(schemaId).Build();
        node.Position = null!;

        var graph = new GraphDtoBuilder()
            .WithNode(node)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<NodePositionRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenNodeDataIsNull_ThrowsNodeDataRequiredException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var node = new NodeDtoBuilder().WithSchemaId(schemaId).Build();
        node.Data = null!;

        var graph = new GraphDtoBuilder()
            .WithNode(node)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<NodeDataRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenNodeTitleIsEmpty_ThrowsNodeTitleRequiredException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var node = new NodeDtoBuilder().WithSchemaId(schemaId).Build();
        node.Data.Title = "";

        var graph = new GraphDtoBuilder()
            .WithNode(node)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<NodeTitleRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenNodeSchemaIdIsEmpty_ThrowsNodeSchemaIdRequiredException()
    {
        var validator = new GraphJsonValidatorService();

        var graph = new GraphDtoBuilder()
            .WithNode(Guid.Empty)
            .Build();

        var schemas = new List<SchemaDto>();

        Assert.Throws<NodeSchemaIdRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenNodeSchemaTypeNameIsEmpty_ThrowsNodeSchemaTypeNameRequiredException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var node = new NodeDtoBuilder()
            .WithSchemaId(schemaId)
            .WithSchemaTypeName("")
            .Build();

        var graph = new GraphDtoBuilder()
            .WithNode(node)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<NodeSchemaTypeNameRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenEdgeIdIsEmpty_ThrowsEdgeIdRequiredException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("node-1", schemaId)
            .WithNode("node-2", schemaId)
            .WithEdge(id: "")
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<EdgeIdRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenEdgeHandleIsEmpty_ThrowsEdgeHandleRequiredException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("node-1", schemaId)
            .WithNode("node-2", schemaId)
            .WithEdge(id: "edge-1")
            .Build();

        graph.Edges[0].SourceHandle = "";

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<EdgeHandleRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenEdgeNoodeIdInvalid_ThrowsInvalidEdgeNodeReferenceException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("node-1", schemaId)
            .WithNode("node-2", schemaId)
            .WithEdge("edge", "node-1", "aboba")
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<InvalidEdgeNodeReferenceException>(() => validator.Validate(graph, schemas));
    }


    [Fact]
    public void Validate_WhenNodeReferencesMissingSchema_ThrowsSchemaNotFoundException()
    {
        var validator = new GraphJsonValidatorService();

        var graph = new GraphDtoBuilder()
            .WithNode("node-1", Guid.NewGuid())
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDto
            {
                Id = Guid.NewGuid(),
                SchemaTypeName = "DialogueNode"
            }
        };

        Assert.Throws<SchemaNotFoundException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenPropertyIsNull_ThrowsInvalidPropertyJsonValueKindException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode(
                new NodeDtoBuilder()
                    .WithSchemaId(schemaId)
                    .WithProperty("field_name", null)
                    .Build()
            ).Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .WithField("field_name", "string")
                .Build()
        };

        Assert.Throws<InvalidPropertyJsonValueKindException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenPropertyTypeMismatch_ThrowsPropertyTypeMismatchException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode(
                new NodeDtoBuilder()
                    .WithSchemaId(schemaId)
                    .WithProperty("string", "somestring")
                    .Build()
            ).Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .WithField("string", "int")
                .Build()
        };

        Assert.Throws<PropertyTypeMismatchException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenPropertyIsNotDefinedInSchema_ThrowsPropertyNotDefinedInSchemaException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode(
                new NodeDtoBuilder()
                    .WithSchemaId(schemaId)
                    .WithProperty("field_name", "string")
                    .Build()
            ).Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .WithField("other_field_name", "string")
                .Build()
        };

        Assert.Throws<PropertyNotDefinedInSchemaException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenGraphNull_ThrowsGraphRequiredException()
    {
        var validator = new GraphJsonValidatorService();

        Assert.Throws<GraphRequiredException>(() => validator.Validate(null!, null!));
    }

    [Fact]
    public void Validate_WhenGraphNodesNull_ThrowsGraphNodesRequiredException()
    {
        var validator = new GraphJsonValidatorService();

        var graph = new GraphDtoBuilder().WithEdge().Build();
        graph.Nodes = null!;

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(Guid.NewGuid())
                .WithField("other_field_name", "string")
                .Build()
        };

        Assert.Throws<GraphNodesRequiredException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenGraphEdgesNull_ThrowsGraphEdgesRequiredException()
    {
        var validator = new GraphJsonValidatorService();

        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder().WithNode(schemaId).Build();

        graph.Edges = null!;

        var schemas = new List<SchemaDto>
        {
            new SchemaDtoBuilder()
                .WithId(schemaId)
                .WithField("other_field_name", "string")
                .Build()
        };

        Assert.Throws<GraphEdgesRequiredException>(() => validator.Validate(graph, schemas));
    }
}
