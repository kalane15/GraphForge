namespace GraphForge.Api.DTOs;

/// <summary>
/// light project info for purposes when graphs are not needed, e.g. for listing projects or managing project metadata
/// </summary>
public record ProjectInfoResponse(Guid Id, string Name, string? Description, int GraphCount);
