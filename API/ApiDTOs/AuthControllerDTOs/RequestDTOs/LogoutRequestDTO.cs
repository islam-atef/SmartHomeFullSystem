namespace API.ApiDTOs.AuthControllerDTOs.RequestDTOs
{
    public class LogoutRequestDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
        public String UserEmail { get; set; } = string.Empty;
    }
}
