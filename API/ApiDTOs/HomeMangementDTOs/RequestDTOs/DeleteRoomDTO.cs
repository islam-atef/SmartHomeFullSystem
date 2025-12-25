namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record DeleteRoomDTO
    {
        public string HomeId { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
    }
}
