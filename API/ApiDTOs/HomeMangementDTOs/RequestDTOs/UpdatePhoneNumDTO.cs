namespace API.ApiDTOs.HomeMangementDTOs.RequestDTOs
{
    public record UpdatePhoneNumDTO
    {
        public string PhoneNumber { get; set; } = default!;
    }
}
