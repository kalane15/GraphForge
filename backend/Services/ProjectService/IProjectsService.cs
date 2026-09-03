using GraphForge.Api.DTOs;

namespace GraphForge.Api.Services.ProjectService;

public interface IProjectsService
{
    Task<ProjectInfoResponse> CreateUserProjectAsync(Guid userId, ProjectInfoEditRequest request);
    Task<List<ProjectInfoResponse>> GetUserProjectsListAsync(Guid userId);
    Task<ProjectDataResponse?> GetUserProjectAsync(Guid userId, Guid projectId);
    Task<ProjectInfoResponse?> UpdateUserProjectAsync(Guid userId, Guid projectId, ProjectInfoEditRequest request);
    Task<bool> DeleteUserProjectAsync(Guid userId, Guid projectId);
}
