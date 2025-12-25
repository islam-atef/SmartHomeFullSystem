namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record AddRoomDTO
    {
        public string HomeId { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
    }
}
