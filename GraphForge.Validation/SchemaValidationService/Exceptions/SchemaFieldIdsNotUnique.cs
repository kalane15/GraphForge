using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;

public class SchemaFieldIdsNotUnique : Exception
{
    public SchemaFieldIdsNotUnique(string message) : base(message)
    {
    }
}
