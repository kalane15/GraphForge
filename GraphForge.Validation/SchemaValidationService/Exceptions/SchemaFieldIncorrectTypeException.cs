using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;
public class SchemaFieldIncorrectTypeException : SchemaValidationException
{
    public SchemaFieldIncorrectTypeException(string message) : base(message)
    {
    }
}
