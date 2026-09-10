using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;

public class SchemaFieldIncorrectNameException : SchemaValidationException
{
    public SchemaFieldIncorrectNameException(string message) : base(message)
    {
    }
}
