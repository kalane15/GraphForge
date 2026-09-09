using GraphForge.Contracts;

namespace GraphForge.Validation;

public interface IGraphJsonValidatorService
{
    void Validate(GraphDto graph, List<SchemaDto> schemas);
}
