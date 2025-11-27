using Web.Models.AuthModel;

namespace Web.Core.Auth
{
    public interface IAuthApi
    {
        Task<AuthResponseDTO?> RefreshAsync(string refreshToken);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO login );
    }
}
