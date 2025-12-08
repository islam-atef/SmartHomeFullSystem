using System.ComponentModel.DataAnnotations;

namespace API.ApiDTOs.DeviceAuthControllerDTOs
{
    public class VerifyDeviceReqDTO
    {
        [Required]
        public Guid OtpQuestionId { get; set; }
        [Required,Range(100000,999999)]
        public int OtpAnswer { get; set; }
    }
}
