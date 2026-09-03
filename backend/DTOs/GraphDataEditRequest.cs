using GraphForge.Api.Models;
using System.Text.Json;

namespace GraphForge.Api.DTOs;

public record GraphDataEditRequest(string Name, JsonDocument Content);
