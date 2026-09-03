namespace GraphForge.Api.Services.ProjectService;

public class ProjectValidationException : Exception
{
    public ProjectValidationException(string message) : base(message)
    {
    }
}
