using System.ComponentModel.DataAnnotations;

namespace Web.Models.UserDeviceModel
{
    public class OtpQuestionAnswerDTO
    {
        public OtpQuestionAnswerDTO() { }
        public OtpQuestionAnswerDTO(Guid? otpQuestionId, int otpAnswer)
        {
            OtpQuestionId = otpQuestionId;
            OtpAnswer = otpAnswer;
        }

        [Required]
        public Guid? OtpQuestionId { get; set; }
        [Required]
        public int OtpAnswer {  get; set; }
    }
}
