using GraphForge.Api.DTOs;

namespace GraphForge.Api.Services;

public interface IProjectsService
{
    Task<ProjectInfoResponse> CreateUserProjectAsync(Guid userId, ProjectInfoEditRequest request);
    Task<ProjectInfoResponse[]> GetUserProjectsAsync(Guid userId);
    Task<ProjectDataResponse?> GetUserProjectAsync(Guid projectId, Guid userId);
    Task<ProjectInfoResponse?> UpdateUserProjectAsync(Guid projectId, Guid userId, ProjectInfoEditRequest request);
    Task<bool> DeleteUserProjectAsync(Guid projectId, Guid userId);
}
