namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record UserDTO
    {
        public string HomeId { get; set; } = String.Empty;
        public string UserId { get; set; } = String.Empty;
    }
}
