using GraphForge.Contracts;

namespace GraphForge.Validation.Tests.TestData;

public sealed class SchemaDtoBuilder
{
    private readonly SchemaDto _schema = new()
    {
        Id = Guid.NewGuid(),
        SchemaTypeName = Guid.NewGuid().ToString()
    };

    public SchemaDtoBuilder WithId(Guid id)
    {
        _schema.Id = id;
        return this;
    }

    public SchemaDtoBuilder WithSchemaTypeName(string schemaTypeName)
    {
        _schema.SchemaTypeName = schemaTypeName;
        return this;
    }

    public SchemaDtoBuilder WithField(string name, string type)
    {
        _schema.Fields.Add(new SchemaFieldDto
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type
        });

        return this;
    }

    public SchemaDto Build()
    {
        return _schema;
    }
}
