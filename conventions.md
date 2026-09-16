# Conventions

## All code must satisfy the following conventions:
- All API endpoints in case of error must return a `ProblemDetails` object.
- Successful command endpoints without returned data should use `NoContent()`.
- Successful query endpoints should return an explicit DTO body. If nothing is found, use `NotFound()` or return an empty DTO/collection, depending on endpoint semantics.
- Custom exception classes must use the `Exception` suffix. Validation-specific exceptions must inherit from the matching domain base type, for example `GraphValidationException` or `SchemaValidationException`.
