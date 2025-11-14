namespace Web.Core.Auth
{
    public interface IAuthApi
    {
        Task<AuthTokens?> RefreshAsync(string refreshToken);
    }
}
