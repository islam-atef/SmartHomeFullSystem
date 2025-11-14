namespace Web.Core.Auth
{
    public interface ITokenStore
    {
        Task<AuthTokens?> GetAsync();
        Task SaveAsync(AuthTokens tokens);
        Task ClearAsync();
    }
}
