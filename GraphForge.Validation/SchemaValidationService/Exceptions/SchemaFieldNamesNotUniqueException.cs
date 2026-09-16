using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;

public class SchemaFieldNamesNotUniqueException : SchemaValidationException
{
    public SchemaFieldNamesNotUniqueException(string message) : base(message)
    {
    }
}
