using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Contracts;

public sealed class SchemaDto
{
    public Guid Id { get; set; }
    public string SchemaTypeName { get; set; } = string.Empty;
    public List<SchemaFieldDto> Fields { get; set; } = new List<SchemaFieldDto>();
}

public sealed record SchemaFieldDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public SchemaFieldDto(Guid id, string name, string type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public SchemaFieldDto()
    {

    }
}
