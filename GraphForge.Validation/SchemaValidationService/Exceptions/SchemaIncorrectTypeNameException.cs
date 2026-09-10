using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;

public class SchemaIncorrectTypeNameException : SchemaValidationException
{
    public SchemaIncorrectTypeNameException(string message) : base(message)
    {
    }
}

