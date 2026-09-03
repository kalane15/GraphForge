namespace GraphForge.Api.Services.UserIdentityProviderService
{
    public interface IUserIdentityProvider
    {
        Guid GetCurrentUserId();
    }
}
