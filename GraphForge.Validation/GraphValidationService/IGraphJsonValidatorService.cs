using GraphForge.Contracts;

namespace GraphForge.Validation.GraphValidationService;

public interface IGraphJsonValidatorService
{
    void Validate(GraphDto graph, List<SchemaDto> schemas);
}
