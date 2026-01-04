using Application.Contracts.Messaging.Mail.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Messaging.mail
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDTO emailDTO);
    }
}
