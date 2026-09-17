using System.ComponentModel.DataAnnotations;
using GraphForge.Contracts;

namespace GraphForge.Api.DTOs.Graphs;

public sealed record UpdateGraphContentRequest(
    [Required] GraphDto Content
);
