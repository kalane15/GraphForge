using System.ComponentModel.DataAnnotations;
using GraphForge.Contracts;

namespace GraphForge.Api.DTOs.Graphs;

public record GraphDataEditRequest(
    [Required] string Name,
    [Required] GraphDto Content
);
