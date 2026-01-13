namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record SubscribeToHomeDTO
    {
        public string homeId { get; init; } = default!;
    }
}
