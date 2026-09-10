using GraphForge.Contracts;

namespace GraphForge.Validation;

public interface ISchemaDtoValidatorService
{
    /// <summary>
    /// Validates schemas list. Schemas in the list MUST BE unique
    /// </summary>
    /// <param name="schemas">Unique list of schemas</param>
    void ValidateSchemasListUnique(List<SchemaDto> schemas);
    void ValidateSchema(SchemaDto schemas);
}
