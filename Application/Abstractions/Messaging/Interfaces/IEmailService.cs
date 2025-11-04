using Application.Abstractions.Messaging.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Messaging.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDTO emailDTO);
    }
}
