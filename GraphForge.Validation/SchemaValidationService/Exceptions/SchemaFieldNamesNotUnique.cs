using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;

public class SchemaFieldNamesNotUnique : SchemaValidationException
{
    public SchemaFieldNamesNotUnique(string message) : base(message)
    {
    }
}
