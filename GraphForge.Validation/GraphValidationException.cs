namespace GraphForge.Validation;

public class GraphValidationException : Exception
{
    public GraphValidationException(string message) : base(message)
    {
    }
}

public sealed class GraphRequiredException : GraphValidationException
{
    public GraphRequiredException(string message) : base(message)
    {
    }
}

public sealed class GraphNodesRequiredException : GraphValidationException
{
    public GraphNodesRequiredException(string message) : base(message)
    {
    }
}

public sealed class GraphEdgesRequiredException : GraphValidationException
{
    public GraphEdgesRequiredException(string message) : base(message)
    {
    }
}

public sealed class NodeIdRequiredException : GraphValidationException
{
    public NodeIdRequiredException(string message) : base(message)
    {
    }
}

public sealed class DuplicateNodeIdException : GraphValidationException
{
    public DuplicateNodeIdException(string message) : base(message)
    {
    }
}

public sealed class EdgeIdRequiredException : GraphValidationException
{
    public EdgeIdRequiredException(string message) : base(message)
    {
    }
}

public sealed class DuplicateEdgeIdException : GraphValidationException
{
    public DuplicateEdgeIdException(string message) : base(message)
    {
    }
}

public sealed class InvalidEdgeNodeReferenceException : GraphValidationException
{
    public InvalidEdgeNodeReferenceException(string message) : base(message)
    {
    }
}

public sealed class SchemaNotFoundException : GraphValidationException
{
    public SchemaNotFoundException(string message) : base(message)
    {
    }
}

public sealed class GraphPropertiesJsonInvalidException : GraphValidationException
{
    public GraphPropertiesJsonInvalidException(string message) : base(message)
    {
    }
}

public sealed class PropertyNameRequiredException : GraphValidationException
{
    public PropertyNameRequiredException(string message) : base(message)
    {
    }
}

public sealed class PropertyNotDefinedInSchemaException : GraphValidationException
{
    public PropertyNotDefinedInSchemaException(string message) : base(message)
    {
    }
}

public sealed class InvalidPropertyJsonValueKindException : GraphValidationException
{
    public InvalidPropertyJsonValueKindException(string message) : base(message)
    {
    }
}

public sealed class PropertyTypeMismatchException : GraphValidationException
{
    public PropertyTypeMismatchException(string message) : base(message)
    {
    }
}
