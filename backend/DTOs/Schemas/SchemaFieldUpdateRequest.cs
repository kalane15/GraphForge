using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Schemas;

public record SchemaFieldUpdateRequest(
    Guid? Id,
    [Required] string Name,
    [Required] string Type
);
