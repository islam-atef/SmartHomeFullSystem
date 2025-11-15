namespace API.ApiDTOs.AuthControllerDTOs.RequestDTOs
{
    public class AccountActivationRequestDTO
    {
        public string UserEmail { get; init; } = default!;
        public string ActivationToken { get; init; } = default!;
    }
}
