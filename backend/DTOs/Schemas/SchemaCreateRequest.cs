using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Schemas;

public sealed record SchemaCreateRequest(
    [Required] string SchemaTypeName,
    [Required] List<SchemaFieldCreationRequest> Fields
);
