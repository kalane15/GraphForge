using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;


namespace GraphForge.Generator;

internal sealed class SchemasFile
{
    public List<Schema> Schemas { get; set; } = new();
}

internal sealed class Schema
{
    public string SchemaTypeName { get; set; } = "";
    public List<Field> Fields { get; set; } = new();
}

internal sealed class Field
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
}

internal sealed class GeneratedSource
{
    public GeneratedSource(string hintName, string source)
    {
        SavePath = hintName;
        Source = source;
    }

    public string SavePath { get; }
    public string Source { get; }
}

internal static class SchemaClassGenerator
{
    private const string parentType = "global::GraphForge.Runtime.GraphNode";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static IReadOnlyList<GeneratedSource> GenerateClassesFromSchemasJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<GeneratedSource>();
        }

        SchemasFile? schemasFile = JsonSerializer.Deserialize<SchemasFile>(json, JsonSerializerOptions);

        if (schemasFile is null)
        {
            return Array.Empty<GeneratedSource>();
        }

        return schemasFile.Schemas
            .Where((schema) => !string.IsNullOrWhiteSpace(schema.SchemaTypeName))
            .Select((schema) => new GeneratedSource(
                $"{schema.SchemaTypeName}.g.cs",
                GenerateClass(schema)))
            .ToList();
    }

    private static string GenerateClass(Schema schema)
    {
        var sourceBuilder = new StringBuilder();

        sourceBuilder.AppendLine("namespace GraphForge.Generated;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine($"public sealed class {schema.SchemaTypeName} : {parentType}");
        sourceBuilder.AppendLine("{");

        foreach (Field field in schema.Fields)
        {
            sourceBuilder.AppendLine($"    public {GetCSharpType(field.Type)} {ToPropertyName(field.Name)} {{ get; set; }}{GetDefaultValue(field.Type)}");
        }

        sourceBuilder.AppendLine("}");

        return sourceBuilder.ToString();
    }

    private static string GetCSharpType(string fieldType)
    {
        return fieldType.ToLowerInvariant() switch
        {
            "bool" => "bool",
            "float" => "float",
            "int" => "int",
            "string" => "string",
            _ => "string"
        };
    }

    private static string GetDefaultValue(string fieldType)
    {
        return fieldType.ToLowerInvariant() switch
        {
            "string" => " = string.Empty;",
            _ => ""
        };
    }

    private static string ToPropertyName(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new InvalidOperationException("Schema field name is required.");
        }

        string[] parts = fieldName
            .Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
        {
            throw new InvalidOperationException("Schema field name is required.");
        }

        return string.Concat(parts.Select(Capitalize));
    }

    private static string Capitalize(string value)
    {
        if (value.Length == 0)
        {
            return value;
        }

        if (value.Length == 1)
        {
            return value.ToUpperInvariant();
        }

        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }
}
