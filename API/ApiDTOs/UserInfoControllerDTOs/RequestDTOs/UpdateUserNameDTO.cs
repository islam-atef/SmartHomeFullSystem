namespace API.ApiDTOs.UserInfoControllerDTOs.RequestDTOs
{
    public record UpdateUserNameDTO
    {
        public string UserName { get; set; } = default!;
    }
}
