using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphForge.Contracts;

public sealed class GraphDto
{
    [JsonPropertyName("nodes")]
    public List<NodeDto> Nodes { get; set; } = new List<NodeDto>();

    [JsonPropertyName("edges")]
    public List<EdgeDto> Edges { get; set; } = new List<EdgeDto>();
}

public sealed class NodeDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public NodeDataDto Data { get; set; } = new NodeDataDto();

    [JsonPropertyName("type")]
    public string ReactFlowType { get; set; } = "editableNode";

    [JsonPropertyName("position")]
    public PositionDto Position { get; set; } = new PositionDto();
}

public sealed class EdgeDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;

    [JsonPropertyName("sourceHandle")]
    public string SourceHandle { get; set; } = string.Empty;

    [JsonPropertyName("targetHandle")]
    public string TargetHandle { get; set; } = string.Empty;
}

public sealed class NodeDataDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("schemaId")]
    public Guid SchemaId { get; set; }

    [JsonPropertyName("schemaTypeName")]
    public string SchemaTypeName { get; set; } = string.Empty;

    [JsonPropertyName("properties")]
    public JsonElement Properties { get; set; }
}

public sealed class PositionDto
{
    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }
}
