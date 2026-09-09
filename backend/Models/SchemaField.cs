using System.ComponentModel.DataAnnotations.Schema;

namespace GraphForge.Api.Models;

[Table("schema_fields")]
public class SchemaField
{
    public Guid Id { get; set; }
    public Guid SchemaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public Schema Schema { get; set; } = null!;
}
