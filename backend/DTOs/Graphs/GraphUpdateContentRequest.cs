using GraphForge.Contracts;
using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Graphs;

public sealed record UpdateGraphContentRequest(
    [Required] GraphDto Content
);
