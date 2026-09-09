using GraphForge.Api.Models;
using System.Text.Json;

namespace GraphForge.Api.DTOs.Graphs;

public record GraphDataEditRequest(string Name, GraphForge.Contracts.GraphDto Content);
