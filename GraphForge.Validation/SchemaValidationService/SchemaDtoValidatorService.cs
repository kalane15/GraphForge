using GraphForge.Contracts;
using GraphForge.Validation.SchemaValidationService;
using GraphForge.Validation.SchemaValidationService.Exceptions;

namespace GraphForge.Validation;

public class SchemaDtoValidatorService : ISchemaDtoValidatorService
{
    /// <summary>
    /// Validates schemas list. Schemas in the list MUST BE unique
    /// </summary>
    /// <param name="schemas">Unique list of schemas</param>
    public void ValidateSchemasListUnique(List<SchemaDto> schemas)
    {
        if (schemas.Select((s) => s.Id).Distinct().Count() != schemas.Count)
        {
            throw new SchemasIdsNotUniqueException("Found duplicate id");
        }

        foreach (SchemaDto schema in schemas)
        {
            ValidateSchema(schema);
        }
    }

    public void ValidateSchema(SchemaDto schema)
    {
        if (string.IsNullOrEmpty(schema.SchemaTypeName))
        {
            throw new SchemaIncorrectTypeNameException("Name is empty");
        }

        if (schema.Fields.Select((s) => s.Id).Distinct().Count() != schema.Fields.Count)
        {
            throw new SchemaFieldIdsNotUnique("Ids of fields inside one schema must be unique");
        }

        if (schema.Fields.Select((s) => s.Name).Distinct().Count() != schema.Fields.Count)
        {
            throw new SchemaFieldNamesNotUnique("Names of fields inside one schema must be unique");
        }

        foreach (SchemaFieldDto fieldDto in schema.Fields)
        {
            if (string.IsNullOrEmpty(fieldDto.Name))
            {
                throw new SchemaFieldIncorrectNameException($"Field name of field with id={fieldDto.Id} is empty.");
            }

            if (string.IsNullOrEmpty(fieldDto.Type))
            {
                throw new SchemaFieldIncorrectTypeException($"Type of field with id={fieldDto.Id} is empty.");
            }

            if (!AllowedTypes.AllowedTypesString.Contains(fieldDto.Type))
            {
                throw new SchemaFieldIncorrectTypeException($"Type of field with id={fieldDto.Id} is incorrect.\n" +
                                                            $"{fieldDto.Type} is not allowed.");
            }
        }
    }
}
