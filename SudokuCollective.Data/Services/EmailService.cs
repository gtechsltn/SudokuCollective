using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Models;

namespace SudokuCollective.Data.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailMetaData emailMetaData;

        public EmailService(IEmailMetaData metaData)
        {
            emailMetaData = (EmailMetaData)metaData;
        }
        
        public bool Send(string to, string subject, string html)
        {
            if (string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));

            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));

            if (string.IsNullOrEmpty(html)) throw new ArgumentNullException(nameof(html));

            // create message
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(emailMetaData.FromEmail));

            email.To.Add(MailboxAddress.Parse(to));

            email.Subject = subject;

            email.Body = new TextPart(TextFormat.Html) { Text = html };

            try
            {
                // send email
                using var smtp = new SmtpClient();

                smtp.Connect(emailMetaData.SmtpServer, emailMetaData.Port, SecureSocketOptions.Auto);

                smtp.Authenticate(emailMetaData.UserName, emailMetaData.Password);

                smtp.Send(email);

                smtp.Disconnect(true);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
