namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record CreateHomeApiDTO
    {
        public required string Name { get; set; } = string.Empty;
        public string? HomeInfo { get; set; } = default!;

        public required double Longitude { get; set; }
        public required double Latitude { get; set; }

        public required string ISO3166_2_lvl4 { get; set; } = default!;
        public required string Country { get; set; } = default!;
        public required string State { get; set; } = default!;
        public required string Address { get; set; } = default!;
    }
}
