using GraphForge.Contracts;
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
            throw new GraphRequiredException("Graph is required.");
        }
        if (graph.Nodes is null)
        {
            throw new GraphNodesRequiredException("Graph nodes are required.");
        }

        if (graph.Edges is null)
        {
            throw new GraphEdgesRequiredException("Graph edges are required.");
        }

        if (graph.Nodes.Any(node => string.IsNullOrWhiteSpace(node.Id)))
        {
            throw new NodeIdRequiredException("Node ids are required.");
        }

        var nodeIds = new HashSet<string>(
            graph.Nodes.Select(node => node.Id));

        if (nodeIds.Count != graph.Nodes.Count)
        {
            throw new DuplicateNodeIdException("Node ids must be unique.");
        }

        if (graph.Edges.Any(edge => string.IsNullOrWhiteSpace(edge.Id)))
        {
            throw new EdgeIdRequiredException("Edge ids are required.");
        }

        var edgeIds = new HashSet<string>(
            graph.Edges.Select(edge => edge.Id));

        if (edgeIds.Count != graph.Edges.Count)
        {
            throw new DuplicateEdgeIdException("Edge ids must be unique.");
        }

        foreach (EdgeDto edge in graph.Edges)
        {
            if (!nodeIds.Contains(edge.Source))
            {
                throw new InvalidEdgeNodeReferenceException(
                    $"Source node '{edge.Source}' does not exist.");
            }

            if (!nodeIds.Contains(edge.Target))
            {
                throw new InvalidEdgeNodeReferenceException(
                    $"Target node '{edge.Target}' does not exist.");
            }
        }

        foreach (NodeDto node in graph.Nodes)
        {
            SchemaDto schema = schemas.FirstOrDefault((schema) => schema.Id == node.Data.SchemaId);
            if (schema == null)
            {
                throw new SchemaNotFoundException(
                    $"Schema {node.Data.SchemaTypeName} not found in schemas provided during validation"
                    );
            }
            ValidateNodeData(node.Data, schema);
        }
    }


    private void ValidateNodeData(NodeDataDto data, SchemaDto schema)
    {
        var properties = new Dictionary<string, JsonElement>();
        try
        {
            properties = data.Properties
                .Deserialize<Dictionary<string, JsonElement>>()
                ?? new Dictionary<string, JsonElement>();
        } catch (Exception ex)
        {
            throw new GraphPropertiesJsonInvalidException($"Cannot parse properties json: {ex.Message}");
        }

        foreach (var property in properties)
        {
            if (property.Key == string.Empty)
            {
                throw new PropertyNameRequiredException("Property names are required.");
            }

            SchemaFieldDto relatedSchemaField = schema.Fields.FirstOrDefault((field) => field.Name == property.Key);

            if (relatedSchemaField == null)
            {
                throw new PropertyNotDefinedInSchemaException($"Property {property.Key} is not defined in provided schema.");
            }

            JsonElement value = property.Value;

            if (ForbiddenJsonTypes.Contains(value.ValueKind))
            {
                throw new InvalidPropertyJsonValueKindException($"Property {property.Key} has forbidden json value kind: {value.ValueKind}");
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

                throw new PropertyTypeMismatchException(
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
                throw new PropertyTypeMismatchException(
                    $"Property {property.Key} value type doesnt match type of corresponding schema field\n" +
                    $"Expected: {relatedSchemaField.Type}\n" +
                    $"Provided: {value.ValueKind}"
                    );
            }

            if (value.ValueKind == JsonValueKind.String && relatedSchemaField.Type != "string")
            {
                throw new PropertyTypeMismatchException(
                    $"Property {property.Key} value type doesnt match type of corresponding schema field\n" +
                    $"Expected: {relatedSchemaField.Type}\n" +
                    $"Provided: {value.ValueKind}"
                    );
            }
        }
    }
}
