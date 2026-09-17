# Conventions

## All code must satisfy the following conventions:
- All API endpoints in case of error must return a `ProblemDetails` object.
- Successful command endpoints without returned data should use `NoContent()`.
- Successful query endpoints should return an explicit DTO body. If nothing is found, use `NotFound()` or return an empty DTO/collection, depending on endpoint semantics.
- Custom exception classes must use the `Exception` suffix. Validation-specific exceptions must inherit from the matching domain base type, for example `GraphValidationException` or `SchemaValidationException`.
- Attempts to access another user's resource must return `404 NotFound` with `ProblemDetails`, not `403 Forbidden`, so the API does not leak whether that resource exists.

## C# formatting

- The root `.editorconfig` defines C# formatting: four spaces, UTF-8, LF line endings, a final newline, no trailing whitespace, file-scoped namespaces, and consistent using and modifier order.
- Keep one blank line between class members and logical blocks; avoid repeated blank lines.
- When splitting a long call, keep `(` on the method-name line, put each argument on its own line, and align the closing `)` with the beginning of the call. Put chained LINQ operations on separate lines.
- Keep unused imports and unused injected constructor dependencies out of the code.
- Run `dotnet format GraphForge.sln --no-restore` after restoring packages. Generated code and EF migrations are excluded from manual style cleanup.
- Verify with `dotnet format GraphForge.sln --verify-no-changes --no-restore`; the same check runs in CI. The formatter does not enforce every manual line-wrapping choice above.
