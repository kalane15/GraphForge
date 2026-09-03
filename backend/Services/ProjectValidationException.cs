namespace GraphForge.Api.Services;

public class ProjectValidationException : Exception
{
    public ProjectValidationException(string message) : base(message)
    {
    }
}
