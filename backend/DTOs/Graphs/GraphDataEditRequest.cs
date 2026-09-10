using GraphForge.Contracts;
using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Graphs;

public record GraphDataEditRequest(
    [Required] string Name,
    [Required] GraphDto Content
);
