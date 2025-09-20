using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace LOJIC.Hangfire.Services.Notifications
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClientOptions _smtpOptions;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _smtpOptions = new SmtpClientOptions();
            _configuration.GetSection(SmtpClientOptions.SmtpClient).Bind(_smtpOptions);
        }

        public void Send(EmailDto email)
        {
            var message = GenerateMessage(email);
                        using var smtpClient = new SmtpClient();

            if (_smtpOptions.UseTls)
                smtpClient.Connect(_smtpOptions.Host, _smtpOptions.Port, SecureSocketOptions.StartTls);
            
            else
                smtpClient.Connect(_smtpOptions.Host, _smtpOptions.Port);
            
            if (_smtpOptions.UseAuthentication)
                smtpClient.Authenticate(_smtpOptions.UserName, _smtpOptions.Password);
            
            smtpClient.Send(message);
            smtpClient.Disconnect(true);
        }

        private MimeMessage GenerateMessage(EmailDto email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Hangfire Automated Notifications", _smtpOptions.From));
            if (!string.IsNullOrWhiteSpace(_smtpOptions.To)) 
                message.To.AddRange(InternetAddressList.Parse(_smtpOptions.To));

            message.Subject = email.Subject;

            var bodyBuilder = new BodyBuilder();
            if (email.IsBodyHtml)
                bodyBuilder.HtmlBody = email.Body;
            else
                bodyBuilder.TextBody = email.Body;
            
            message.Body = bodyBuilder.ToMessageBody();
            return message;
        }

        public class SmtpClientOptions
        {
            public const string SmtpClient = "SMTPNotifications";

            public string Host { get; set; }
            public int Port { get; set; }
            public bool UseAuthentication { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool UseTls { get; set; }
            public bool Enabled { get; set; }
            public string From { get; set; }
            public string To { get; set; }
        }

        public class EmailDto
        {
            public string To { get; set; }
            public string Cc { get; set; }
            public string Bcc { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsBodyHtml { get; set; }
        }
    }
}
