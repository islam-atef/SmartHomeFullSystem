namespace API.ApiDTOs.AuthControllerDTOs.RequestDTOs
{
    public class LoginReqDTO
    {
        public string? Email { get; init; } = default;
        public string? Username { get; init; } = default;
        public string Password { get; init; } = default!;
    }
}
