using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;

public class SchemaFieldIdsNotUnique : SchemaValidationException
{
    public SchemaFieldIdsNotUnique(string message) : base(message)
    {
    }
}
