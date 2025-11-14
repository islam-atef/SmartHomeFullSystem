namespace Web.Models.AuthModel
{
    public record AuthResponseDTO(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);
}
