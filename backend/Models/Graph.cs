using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace GraphForge.Api.Models;

[Table("graphs")]
public class Graph
{
    private const string DefaultContentJson = "{\"nodes\":[],\"edges\":[]}";

    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public JsonDocument Content { get; set; } = JsonDocument.Parse(DefaultContentJson);
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Project Project { get; set; } = null!;
}
