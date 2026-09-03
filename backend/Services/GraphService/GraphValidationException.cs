namespace GraphForge.Api.Services.GraphService;

public class IncorrectProjectOwnerException : Exception
{
    public IncorrectProjectOwnerException(string message) : base(message)
    {
    }
}
