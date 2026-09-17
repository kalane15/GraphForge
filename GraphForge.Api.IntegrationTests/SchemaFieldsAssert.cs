using GraphForge.Api.DTOs.Schemas;

namespace GraphForge.Api.IntegrationTests;

internal static class SchemaFieldsAssert
{
    public static void AssertSchemaFieldsEqual(
        IReadOnlyCollection<SchemaFieldCreationRequest> expectedFields,
        IReadOnlyCollection<SchemaFieldDefinitionResponse> actualFields)
    {
        AssertSchemaFieldsEqual(
            expectedFields.Select(field => (field.Name, field.Type)),
            actualFields);
    }

    public static void AssertSchemaFieldsEqual(
        IReadOnlyCollection<SchemaFieldUpdateRequest> expectedFields,
        IReadOnlyCollection<SchemaFieldDefinitionResponse> actualFields)
    {
        AssertSchemaFieldsEqual(
            expectedFields.Select(field => (field.Name, field.Type)),
            actualFields);
    }

    private static void AssertSchemaFieldsEqual(
        IEnumerable<(string Name, string Type)> expectedFields,
        IReadOnlyCollection<SchemaFieldDefinitionResponse> actualFields)
    {
        Assert.Equal(expectedFields.Count(), actualFields.Count);

        foreach ((string expectedName, string expectedType) in expectedFields)
        {
            SchemaFieldDefinitionResponse? actualField =
                actualFields.FirstOrDefault(field => field.Name == expectedName);

            Assert.NotNull(actualField);
            Assert.Equal(expectedType, actualField.Type);
            Assert.NotEqual(Guid.Empty, actualField.Id);
        }
    }
}
