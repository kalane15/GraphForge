namespace GraphForge.Api.Services;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}