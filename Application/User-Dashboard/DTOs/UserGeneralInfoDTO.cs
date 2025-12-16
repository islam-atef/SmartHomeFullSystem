using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User_Dashboard.DTOs
{
    public class UserGeneralInfoDTO
    {
        public UserGeneralInfoDTO(string name, string email, string userName, string phoneNumber, string userImageUrl)
        {
            Name = name;
            Email = email;
            UserName = userName;
            PhoneNumber = phoneNumber;
            UserImageUrl = userImageUrl;
        }

        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string UserImageUrl { get; set; } = default!;
    }
}
