using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;

public class SchemaFieldIdsNotUniqueException : SchemaValidationException
{
    public SchemaFieldIdsNotUniqueException(string message) : base(message)
    {
    }
}
