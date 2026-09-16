using GraphForge.Api.DTOs.Schemas;
using GraphForge.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Api.IntegrationTests.SchemasControllerTests;

internal class RandomFieldsFactory
{
    public static string GetRandomType()
    {
        int index = Random.Shared.Next(AllowedSchemaFieldTypes.AllowedTypesString.Count);
        return AllowedSchemaFieldTypes.AllowedTypesString[index];
    }

    public static List<SchemaFieldCreationRequest> GetFieldsRandomValidData(int count = 5)
    {
        List<SchemaFieldCreationRequest> result = new List<SchemaFieldCreationRequest>();

        for (int i = 0; i < count; i++)
        {
            string type = GetRandomType();
            SchemaFieldCreationRequest def = new SchemaFieldCreationRequest($"field{i}", type);
            result.Add(def);
        }

        return result;
    }
}
