namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record UserDTO
    {
        public string HomeId { get; set; } = String.Empty;
        public string NewUserId { get; set; } = String.Empty;
    }
}
