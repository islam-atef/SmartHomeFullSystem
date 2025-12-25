namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record RenameApiDTO
    {
        public string NewName { get; set; } = string.Empty;
        public string HomeId { get; set; }
    }
}
