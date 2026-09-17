using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Schemas;

public sealed record SchemaEditDataRequest(
    [Required] string SchemaTypeName,
    [Required] List<SchemaFieldUpdateRequest> Fields
);
