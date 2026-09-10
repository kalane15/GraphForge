using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Graphs;

public record GraphCreationRequest(
    [Required] string Name
);
