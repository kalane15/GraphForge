using GraphForge.Api.DTOs;
using System.Text.Json;

namespace GraphForge.Api.Services.GraphService
{
    public interface IGraphsService
    {
        Task<GraphInfoResponse> CreateUserGraphAsync(Guid userId, Guid projectId, GraphCreationRequest request);
        Task<List<GraphInfoResponse>> GetUserProjectsGraphsAsync(Guid userId, Guid projectId);
        Task<GraphDataResponse> GetUserGraphAsync(Guid userId, Guid projectId, Guid graphId);
        Task<GraphDataResponse> UpdateUserGraphAsync(Guid userId, Guid projectId, Guid graphId, GraphDataEditRequest request);
        Task DeleteUserGraphAsync(Guid userId, Guid projectId, Guid graphId);

        Task UpdateUserGraphContentAsync(Guid userId, Guid projectId, Guid graphId, JsonDocument content);
    }
}
