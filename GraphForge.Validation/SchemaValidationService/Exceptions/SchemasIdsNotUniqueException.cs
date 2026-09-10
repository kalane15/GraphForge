using System;
using System.Collections.Generic;
using System.Text;

namespace GraphForge.Validation.SchemaValidationService.Exceptions;
public class SchemasIdsNotUniqueException : SchemaValidationException
{
    public SchemasIdsNotUniqueException(string message) : base(message)
    {
    }
}
