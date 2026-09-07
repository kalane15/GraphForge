using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace GraphForge.Api.Models;

[Table("schemas")]
public class Schema
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string SchemaTypeName { get; set; } = string.Empty;
    public JsonDocument Content { get; set; } = JsonDocument.Parse("""{"fields":[]}""");
    public Project Project { get; set; } = null!;
}
