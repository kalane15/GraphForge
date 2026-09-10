namespace GraphForge.Api.Services.AuthService;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string message) : base(message)
    {
    }
}
