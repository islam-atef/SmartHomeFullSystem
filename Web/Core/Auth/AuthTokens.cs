namespace Web.Core.Auth
{
    public record AuthTokens(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);
}
