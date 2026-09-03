using System.Text.Json;

namespace GraphForge.Api.DTOs;

public record GraphDataResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    JsonDocument Content,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
