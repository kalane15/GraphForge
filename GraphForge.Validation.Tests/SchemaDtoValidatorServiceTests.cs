using GraphForge.Contracts;
using GraphForge.Validation.SchemaValidationService.Exceptions;
using SchemaBuilder = GraphForge.Validation.Tests.SchemaDtoBuilder;

namespace GraphForge.Validation.Tests;

public sealed class SchemaDtoValidatorServiceTests
{
    [Fact]
    public void ValidateSchemasListUnique_WhenSchemaIdsAreDuplicated_ThrowsSchemasIdsNotUniqueException()
    {
        var validator = new SchemaDtoValidatorService();
        Guid schemaId = Guid.NewGuid();

        var schemas = new List<SchemaDto>
        {
            new SchemaBuilder()
                .WithId(schemaId)
                .Build(),
            new SchemaBuilder()
                .WithId(schemaId)
                .Build()
        };

        Assert.Throws<SchemasIdsNotUniqueException>(() => validator.ValidateSchemasListUnique(schemas));
    }

    [Fact]
    public void ValidateSchema_WhenSchemaTypeNameIsEmpty_ThrowsSchemaIncorrectTypeNameException()
    {
        var validator = new SchemaDtoValidatorService();

        SchemaDto schema = new SchemaBuilder()
            .WithSchemaTypeName("")
            .Build();

        Assert.Throws<SchemaIncorrectTypeNameException>(() => validator.ValidateSchema(schema));
    }

    [Theory]
    [InlineData("dialogueNode")]
    [InlineData("Dialogue Node")]
    [InlineData("Dialogue_Node")]
    [InlineData("1DialogueNode")]
    [InlineData("DialogueNode!")]
    public void ValidateSchema_WhenSchemaTypeNameIsNotPascalCase_ThrowsSchemaIncorrectTypeNameException(
        string schemaTypeName)
    {
        var validator = new SchemaDtoValidatorService();

        SchemaDto schema = new SchemaBuilder()
            .WithSchemaTypeName(schemaTypeName)
            .Build();

        Assert.Throws<SchemaIncorrectTypeNameException>(() => validator.ValidateSchema(schema));
    }

    [Fact]
    public void ValidateSchema_WhenFieldIdsAreDuplicated_ThrowsSchemaFieldIdsNotUnique()
    {
        var validator = new SchemaDtoValidatorService();
        Guid fieldId = Guid.NewGuid();

        SchemaDto schema = new SchemaBuilder()
            .WithField(fieldId, "firstField", "string")
            .WithField(fieldId, "secondField", "int")
            .Build();

        Assert.Throws<SchemaFieldIdsNotUnique>(() => validator.ValidateSchema(schema));
    }

    [Fact]
    public void ValidateSchema_WhenFieldNamesAreDuplicated_ThrowsSchemaFieldNamesNotUnique()
    {
        var validator = new SchemaDtoValidatorService();

        SchemaDto schema = new SchemaBuilder()
            .WithField("sameField", "string")
            .WithField("sameField", "int")
            .Build();

        Assert.Throws<SchemaFieldNamesNotUnique>(() => validator.ValidateSchema(schema));
    }

    [Fact]
    public void ValidateSchema_WhenFieldNameIsEmpty_ThrowsSchemaFieldIncorrectNameException()
    {
        var validator = new SchemaDtoValidatorService();

        SchemaDto schema = new SchemaBuilder()
            .WithField("", "string")
            .Build();

        Assert.Throws<SchemaFieldIncorrectNameException>(() => validator.ValidateSchema(schema));
    }

    [Theory]
    [InlineData("SpeakerName")]
    [InlineData("speaker_name")]
    [InlineData("speaker-name")]
    [InlineData("speaker name")]
    [InlineData("1speaker")]
    [InlineData("speaker!")]
    public void ValidateSchema_WhenFieldNameIsNotCamelCase_ThrowsSchemaFieldIncorrectNameException(
        string fieldName)
    {
        var validator = new SchemaDtoValidatorService();

        SchemaDto schema = new SchemaBuilder()
            .WithField(fieldName, "string")
            .Build();

        Assert.Throws<SchemaFieldIncorrectNameException>(() => validator.ValidateSchema(schema));
    }

    [Fact]
    public void ValidateSchema_WhenFieldTypeIsEmpty_ThrowsSchemaFieldIncorrectTypeException()
    {
        var validator = new SchemaDtoValidatorService();

        SchemaDto schema = new SchemaBuilder()
            .WithField("field", "")
            .Build();

        Assert.Throws<SchemaFieldIncorrectTypeException>(() => validator.ValidateSchema(schema));
    }

    [Fact]
    public void ValidateSchema_WhenFieldTypeIsNotAllowed_ThrowsSchemaFieldIncorrectTypeException()
    {
        var validator = new SchemaDtoValidatorService();

        SchemaDto schema = new SchemaBuilder()
            .WithField("field", "date")
            .Build();

        Assert.Throws<SchemaFieldIncorrectTypeException>(() => validator.ValidateSchema(schema));
    }
}
