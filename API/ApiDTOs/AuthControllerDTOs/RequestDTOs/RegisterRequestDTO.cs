namespace API.ApiDTOs.AuthControllerDTOs.RequestDTOs
{
    public class RegisterRequestDTO
    {
        public string Email { get; init; } = default!;
        public string Username { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string? DisplayName { get; init; }
    }
}
