using GraphForge.Contracts;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.Json;

namespace GraphForge.Validation;

public class GraphJsonValidatorService : IGraphJsonValidatorService
{
    private static readonly IReadOnlyList<JsonValueKind> ForbiddenJsonTypes = new List<JsonValueKind>() {
        JsonValueKind.Null,
        JsonValueKind.Object,
        JsonValueKind.Undefined,
        JsonValueKind.Array
    };


    public void Validate(GraphDto graph, List<SchemaDto> schemas)
    {
        if (graph == null)
        {
            throw new GraphValidationException("Graph is null");
        }
        if(graph.Nodes is null)
        {
            throw new GraphValidationException("Graph nodes are required.");
        }

        if (graph.Edges is null)
        {
            throw new GraphValidationException("Graph edges are required.");
        }

        var nodeIds = new HashSet<string>(
            graph.Nodes.Where(node => node.Id != null).Select(node => node.Id));

        if (nodeIds.Count != graph.Nodes.Count)
        {
            throw new GraphValidationException("Nodes ids must be unique");
        }

        var edgeIds = new HashSet<string>(
            graph.Edges.Where(edge => edge.Id != null).Select(edge => edge.Id));

        if (edgeIds.Count != graph.Edges.Count)
        {
            throw new GraphValidationException("Edges ids must be unique");
        }

        foreach (EdgeDto edge in graph.Edges)
        {
            if (!nodeIds.Contains(edge.Source))
            {
                throw new GraphValidationException(
                    $"Source node '{edge.Source}' does not exist.");
            }

            if (!nodeIds.Contains(edge.Target))
            {
                throw new GraphValidationException(
                    $"Target node '{edge.Target}' does not exist.");
            }
        }

        foreach (NodeDto node in graph.Nodes)
        {
            SchemaDto schema = schemas.FirstOrDefault((schema) => schema.Id == node.Data.SchemaId);
            if (schema == null)
            {
                throw new GraphValidationException(
                    $"Schema {node.Data.SchemaTypeName} not found in schemas provided during validation"
                    );
            }
            ValidateNodeData(node.Data, schema);
        }
    }


    private void ValidateNodeData(NodeDataDto data, SchemaDto schema)
    {
        var properties = data.Properties
            .Deserialize<Dictionary<string, JsonElement>>()
            ?? new Dictionary<string, JsonElement>();

        foreach (var property in properties)
        {
            if (property.Key == string.Empty)
            {
                throw new GraphValidationException("Empty field names are not allowed");
            }

            SchemaFieldDto relatedSchemaField = schema.Fields.FirstOrDefault((field) => field.Name == property.Key);

            if (relatedSchemaField == null)
            {
                throw new GraphValidationException($"Property {property.Key} is not present in provided schema");
            }

            JsonElement value = property.Value;

            if (ForbiddenJsonTypes.Contains(value.ValueKind))
            {
                throw new GraphValidationException($"Property {property.Key} has forbidden json value kind: {value.ValueKind}");
            }

            if (value.ValueKind == JsonValueKind.Number)
            {
                if (value.TryGetInt32(out int intValue))
                {
                    if (relatedSchemaField.Type == "int")
                    {
                        continue;
                    }
                }


                if (value.TryGetDouble(out double doubleValue))
                {
                    if (relatedSchemaField.Type == "float")
                    {
                        continue;
                    }
                }

                throw new GraphValidationException(
                        $"Property {property.Key} value type doesnt match type of corresponding schema field\n" +
                        $"Expected: {relatedSchemaField.Type}\n" +
                        $"Provided: {value.ValueKind}"
                        );
            }

            if (value.ValueKind == JsonValueKind.True ||
                value.ValueKind == JsonValueKind.False)
            {
                if (relatedSchemaField.Type == "bool")
                {
                    continue;
                }
                throw new GraphValidationException(
                    $"Property {property.Key} value type doesnt match type of corresponding schema field\n" +
                    $"Expected: {relatedSchemaField.Type}\n" +
                    $"Provided: {value.ValueKind}"
                    );
            }

            if (value.ValueKind == JsonValueKind.String && relatedSchemaField.Type != "string")
            {
                throw new GraphValidationException(
                    $"Property {property.Key} value type doesnt match type of corresponding schema field\n" +
                    $"Expected: {relatedSchemaField.Type}\n" +
                    $"Provided: {value.ValueKind}"
                    );
            }
        }
    }
}
