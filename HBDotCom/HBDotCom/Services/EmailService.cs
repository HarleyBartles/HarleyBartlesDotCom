using HBDotCom.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBDotCom.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailServerConfiguration _config;
        public EmailService(EmailServerConfiguration config)
        {
            _config = config;
        }
        public void Send(EmailMessage msg)
        {
            var message = new MimeMessage();
            message.To.AddRange(msg.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(msg.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            message.Subject = msg.Subject;

            message.Body = new TextPart("plain")
            {
                Text = msg.Content
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(_config.SmtpServer, _config.SmtpPort);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(_config.SmtpUsername, _config.SmtpPassword);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
    public interface IEmailService
    {
        void Send(EmailMessage msg);
    }
}
