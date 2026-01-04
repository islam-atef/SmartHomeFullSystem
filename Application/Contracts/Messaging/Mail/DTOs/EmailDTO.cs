using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Messaging.Mail.DTOs
{
    public class EmailDTO
    {
        public EmailDTO(string to, string from, string subject, string content)
        {
            To = to;
            From = from;
            Subject = subject;
            Content = content;
        }
        public string To { get; init; }
        public string From { get; init; }
        public string Subject { get; init; }
        public string Content { get; init; }
    }
}
