using System.Text.Json;

namespace GraphForge.Api.Services.GraphJsonValidatorService;

public interface IGraphJsonValidatorService
{
    void Validate(GraphForge.Contracts.GraphDto? graph);
}
