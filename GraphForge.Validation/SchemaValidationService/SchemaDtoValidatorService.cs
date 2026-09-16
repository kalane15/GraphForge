using GraphForge.Contracts;
using GraphForge.Validation.SchemaValidationService;
using GraphForge.Validation.SchemaValidationService.Exceptions;
using System.Text.RegularExpressions;

namespace GraphForge.Validation;

public class SchemaDtoValidatorService : ISchemaDtoValidatorService
{
    private static readonly Regex CamelCaseRegex = new("^[a-z][A-Za-z0-9]*$");
    private static readonly Regex PascalCaseRegex = new("^[A-Z][A-Za-z0-9]*$");

    /// <summary>
    /// Validates schemas list. Schemas in the list MUST BE unique
    /// </summary>
    /// <param name="schemas">Unique list of schemas</param>
    public void ValidateSchemasListUnique(List<SchemaDto> schemas)
    {
        if (schemas == null)
        {
            throw new SchemaValidationException("Schema list is null");
        }

        if (schemas.Count((s) => s is null) > 0)
        {
            throw new SchemaValidationException("Schemas list contains null values");
        }

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
        if (schema == null)
        {
            throw new SchemaValidationException("Schema is null");
        }

        if (schema.Fields is null)
        {
            throw new SchemaValidationException("Fields are required");
        }

        if (schema.Fields.Any(field => field is null))
        {
            throw new SchemaValidationException("Fields cannot contain null values");
        }

        if (schema.Id == Guid.Empty)
        {
            throw new SchemaValidationException("Schema id is empty");
        }

        if (string.IsNullOrWhiteSpace(schema.SchemaTypeName))
        {
            throw new SchemaIncorrectTypeNameException("Name is empty");
        }

        if (!PascalCaseRegex.IsMatch(schema.SchemaTypeName))
        {
            throw new SchemaIncorrectTypeNameException($"SchemaTypeName={schema.SchemaTypeName} is not in PascalCase" +
                                                       $"or contains incorrect characters");
        }

        if (schema.Fields.Select((s) => s.Id).Distinct().Count() != schema.Fields.Count)
        {
            throw new SchemaFieldIdsNotUniqueException("Ids of fields inside one schema must be unique");
        }

        if (schema.Fields.Select((s) => s.Name).Distinct().Count() != schema.Fields.Count)
        {
            throw new SchemaFieldNamesNotUniqueException("Names of fields inside one schema must be unique");
        }

        foreach (SchemaFieldDto fieldDto in schema.Fields)
        {

            if (fieldDto.Id == Guid.Empty)
            {
                throw new SchemaValidationException("Field id is empty");
            }

            if (string.IsNullOrWhiteSpace(fieldDto.Name))
            {
                throw new SchemaFieldIncorrectNameException($"Field name of field with id={fieldDto.Id} is empty.");
            }

            if (!CamelCaseRegex.IsMatch(fieldDto.Name))
            {
                throw new SchemaFieldIncorrectNameException($"Field name of field with id={fieldDto.Id} is not in camelCase, " +
                                                            $"or contains incorrect characters");
            }

            if (string.IsNullOrWhiteSpace(fieldDto.Type))
            {
                throw new SchemaFieldIncorrectTypeException($"Type of field with id={fieldDto.Id} is empty.");
            }

            if (!AllowedSchemaFieldTypes.AllowedTypesString.Contains(fieldDto.Type))
            {
                throw new SchemaFieldIncorrectTypeException($"Type of field with id={fieldDto.Id} is incorrect.\n" +
                                                            $"{fieldDto.Type} is not allowed.");
            }
        }
    }
}
