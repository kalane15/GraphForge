namespace GraphForge.Api.DTOs;

//Id is uuid by which project is accessible via api
public record ProjectInfo(Guid Id, string Name, string? Description);

public record ProjectsListResponse(ProjectInfo[] Projects);