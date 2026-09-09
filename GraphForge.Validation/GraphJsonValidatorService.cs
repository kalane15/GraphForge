using GraphForge.Contracts;
using System.Linq;
using System.Text.Json;

namespace GraphForge.Validation;

public class GraphJsonValidatorService : IGraphJsonValidatorService
{
    public void Validate(GraphDto graph, List<SchemaDto> schemas)
    {
        if(graph.Nodes is null)
        {
            throw new GraphValidationException("Graph nodes are required.");
        }

        if (graph.Edges is null)
        {
            throw new GraphValidationException("Graph edges are required.");
        }

        var nodeIds = new HashSet<string>(
            graph.Nodes.Select(node => node.Id));

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
            SchemaDto schema = schemas.FirstOrDefault((schema) => schema.SchemaTypeName == node.Data.SchemaTypeName);
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

            if (value.ValueKind == JsonValueKind.Number)
            {
                if (value.TryGetInt32(out int intValue))
                {
                    if (relatedSchemaField.Type == "int")
                    {
                        return;
                    }
                }


                if (value.TryGetDouble(out double doubleValue))
                {
                    if (relatedSchemaField.Type == "float")
                    {
                        return;
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
                    return;
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
