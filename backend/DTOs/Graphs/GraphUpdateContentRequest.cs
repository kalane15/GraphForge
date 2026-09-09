using System.Text.Json;

namespace GraphForge.Api.DTOs.Graphs;

public sealed record UpdateGraphContentRequest(GraphForge.Contracts.GraphDto Content);
