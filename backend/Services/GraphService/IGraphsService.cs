using GraphForge.Api.DTOs;

namespace GraphForge.Api.Services.GraphService
{
    public interface IGraphsService
    {
        Task<GraphInfoResponse> CreateUserGraphAsync(Guid userId, Guid projectId, GraphCreationRequest request);
        Task<List<GraphInfoResponse>> GetUserProjectsGraphsAsync(Guid userId, Guid projectId);
        Task<GraphDataResponse?> GetUserGraphAsync(Guid userId, Guid projectId, Guid graphId);
        Task<GraphDataResponse?> UpdateUserGraphAsync(Guid userId, Guid projectId, Guid graphId, GraphDataEditRequest request);
        Task<bool> DeleteUserGraphAsync(Guid userId, Guid projectId, Guid graphId);
    }
}
