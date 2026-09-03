namespace GraphForge.Api.Services.GraphService;

public class GraphValidationException : Exception
{
    public GraphValidationException(string message) : base(message)
    {
    }
}
