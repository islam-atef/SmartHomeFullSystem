namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record UpdateImageDTO
    {
        public IFormFile Image { get; set; } = default!;
    }
}
