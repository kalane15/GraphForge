using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;

public class SchemaValidationException : Exception
{
    public SchemaValidationException(string message) : base(message)
    {
    }
}
