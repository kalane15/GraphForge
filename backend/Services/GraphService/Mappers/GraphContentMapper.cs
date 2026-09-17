using GraphForge.Contracts;
using GraphForge.Validation.GraphValidationService;
using System.Text.Json;

namespace GraphForge.Api.Services.GraphService.Mappers;

internal static class GraphContentMapper
{
    public static GraphDto ToDto(JsonDocument content)
    {
        GraphDto? graphDto = content.RootElement.Deserialize<GraphDto>();

        if (graphDto is null)
        {
            throw new GraphValidationException("Graph content is invalid");
        }

        return graphDto;
    }

    public static JsonDocument ToJsonDocument(GraphDto content)
    {
        return JsonSerializer.SerializeToDocument(content);
    }
}
