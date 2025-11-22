using System.ComponentModel.DataAnnotations;

namespace API.ApiDTOs.DeviceAuthControllerDTOs
{
    public class VerifyDeviceReqDTO
    {
        [Required]
        public Guid OtoQuestionId { get; set; }
        [Required,Range(100000,999999)]
        public int OtpAnswer { get; set; }
    }
}
