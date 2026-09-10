namespace GraphForge.Api.Services.AuthService;

public class UnauthorizedUserException : Exception
{
    public UnauthorizedUserException(string message) : base(message)
    {
    }
}
