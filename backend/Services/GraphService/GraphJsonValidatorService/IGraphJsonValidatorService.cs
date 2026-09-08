using System.Text.Json;

namespace GraphForge.Api.Services.GraphService.GraphJsonValidatorService;

public interface IGraphJsonValidatorService
{
    void Validate(JsonDocument graphJson);
}
