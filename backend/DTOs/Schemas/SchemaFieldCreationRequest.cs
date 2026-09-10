using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Schemas;

public sealed record SchemaFieldCreationRequest(
    [Required] string Name,
    [Required] string Type
);
