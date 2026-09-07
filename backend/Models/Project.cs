using System.ComponentModel.DataAnnotations.Schema;

namespace GraphForge.Api.Models;

[Table("projects")]
public class Project
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public User Owner { get; set; } = null!;
    public List<Graph> Graphs { get; set; } = new List<Graph>();
    public List<Schema> Schemas { get; set; } = new List<Schema>();
}
