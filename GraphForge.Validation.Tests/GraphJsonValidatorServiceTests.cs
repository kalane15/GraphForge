using GraphForge.Contracts;
using GraphForge.Validation.Tests.TestData;

namespace GraphForge.Validation.Tests;

public class GraphJsonValidatorServiceTests
{
    [Fact]
    public void Validate_WhenNodeIdsAreDuplicated_ThrowsGraphValidationException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("node-1", schemaId)
            .WithNode("node-1", schemaId)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDto
            {
                Id = schemaId,
                SchemaTypeName = "DialogueNode"
            }
        };

        Assert.Throws<GraphValidationException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenEdgesIdsAreDuplicated_ThrowsGraphValidationException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("node-1", schemaId)
            .WithNode("node-2", schemaId)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDto
            {
                Id = schemaId,
                SchemaTypeName = "DialogueNode"
            }
        };

        Assert.Throws<GraphValidationException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenNodesIdsAreEmpty_ThrowsGraphValidationException()
    {
        var validator = new GraphJsonValidatorService();
        var schemaId = Guid.NewGuid();

        var graph = new GraphDtoBuilder()
            .WithNode("", schemaId)
            .Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDto
            {
                Id = schemaId,
                SchemaTypeName = "DialogueNode"
            }
        };

        Assert.Throws<GraphValidationException>(() => validator.Validate(graph, schemas));
    }


    [Fact]
    public void Validate_WhenNodeRefNotExistingSchema_ThrowsGraphValidationException()
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

        Assert.Throws<GraphValidationException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenPropertyHasNull_ThrowsGraphValidationException()
    {
        var validator = new GraphJsonValidatorService();

        var graph = new GraphDtoBuilder()
            .WithNode(
                new NodeDtoBuilder()
                .WithProperty("string", null)
                .Build()
            ).Build();

        var schemas = new List<SchemaDto>
        {
            new SchemaDto
            {
                Id = Guid.NewGuid(),
                SchemaTypeName = "DialogueNode"
            }
        };

        Assert.Throws<GraphValidationException>(() => validator.Validate(graph, schemas));
    }

    [Fact]
    public void Validate_WhenPropertyDoesNotExistInSchema_ThrowsGraphValidationException()
    {
        var validator = new GraphJsonValidatorService();

        var graph = new GraphDtoBuilder()
            .WithNode(
                new NodeDtoBuilder()
                .WithProperty("field_name", "string")
                .Build()
            ).Build();

        var schemas = new List<SchemaDto>
        {
           new SchemaDtoBuilder().WithField("other_field_name", "string").Build()
        };

        Assert.Throws<GraphValidationException>(() => validator.Validate(graph, schemas));
    }
}
