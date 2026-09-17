using System.ComponentModel.DataAnnotations.Schema;

namespace GraphForge.Api.Models;

[Table("schemas")]
public class Schema
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string SchemaTypeName { get; set; } = string.Empty;
    public List<SchemaField> Fields { get; set; } = new List<SchemaField>();
    public Project Project { get; set; } = null!;
}
