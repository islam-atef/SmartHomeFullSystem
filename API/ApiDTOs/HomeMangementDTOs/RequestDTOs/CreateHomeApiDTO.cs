namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record CreateHomeApiDTO
    {
        public string Name { get; set; } = string.Empty;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
