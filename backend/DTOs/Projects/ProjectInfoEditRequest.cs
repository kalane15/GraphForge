using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Projects;

public record ProjectInfoEditRequest(
    [Required] string Name,
    string? Description
);
