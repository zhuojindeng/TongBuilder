﻿using MimeKit.Text;
using MimeKit;
using TongBuilder.Contract.Plugins;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;

namespace TongBuilder.Messager.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> logger;
        private readonly SmtpOptions smtpOptions;

        public EmailSender(SmtpOptions smtpOptions, ILogger<EmailSender> logger)
        {
            this.smtpOptions = smtpOptions;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string sendTo, string subject, string htmlMessage)
        {
            try
            {
                using (var smtp = new SmtpClient())
                {
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(smtpOptions.From));
                    email.To.Add(MailboxAddress.Parse(sendTo));
                    email.Subject = subject;
                    email.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

                    await smtp.ConnectAsync(smtpOptions.Host, int.Parse(smtpOptions.Port));
                    await smtp.AuthenticateAsync(smtpOptions.UserName, smtpOptions.Password);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"There was an error sending email to {sendTo} with subject {subject}");
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
