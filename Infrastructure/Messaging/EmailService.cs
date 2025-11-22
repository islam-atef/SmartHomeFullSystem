using Application.Abstractions.Messaging.DTOs;
using Application.Abstractions.Messaging.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task SendEmailAsync(EmailDTO emailDTO)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("SmartHomeSystem", configuration["EmailSetting:From"]));
            message.Subject = emailDTO.Subject;
            message.To.Add(new MailboxAddress(emailDTO.To, emailDTO.To));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDTO.Content
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(configuration["EmailSetting:SmtpHost"],
                        int.Parse(configuration["EmailSetting:Port"]!),
                        true);
                    await smtp.AuthenticateAsync(configuration["EmailSetting:UserName"], configuration["EmailSetting:Password"]);
                    await smtp.SendAsync(message);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to send email", ex);
                }
                finally
                {
                    smtp.Disconnect(true);
                    smtp.Dispose();
                }
            }
        }
    }
}
