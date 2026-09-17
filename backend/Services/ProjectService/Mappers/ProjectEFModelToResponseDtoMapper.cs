using GraphForge.Api.DTOs.Graphs;
using GraphForge.Api.DTOs.Projects;
using GraphForge.Api.Models;

namespace GraphForge.Api.Services.ProjectService.Mappers;

internal class ProjectEFModelToResponseDtoMapper
{
    public static ProjectInfoResponse ToInfoResponse(Project project, int graphCount)
    {
        return new ProjectInfoResponse(
            project.Id,
            project.Name,
            project.Description,
            graphCount
        );
    }

    public static ProjectDataResponse ToDataResponse(Project project, List<GraphInfoResponse> graphs)
    {
        return new ProjectDataResponse(
            project.Id,
            project.OwnerId,
            project.Name,
            project.Description,
            project.CreatedAt,
            project.UpdatedAt,
            graphs
        );
    }
}
