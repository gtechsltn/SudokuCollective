using System;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Messages;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Utilities;

namespace SudokuCollective.Data.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailMetaData emailMetaData;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEmailMetaData metaData, 
            ILogger<EmailService> logger)
        {
            emailMetaData = (EmailMetaData)metaData;
            _logger = logger;
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
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(emailMetaData.SmtpServer, emailMetaData.Port, SecureSocketOptions.Auto);

                    smtp.Authenticate(emailMetaData.UserName, emailMetaData.Password);

                    var smtpResponse = smtp.Send(email);

                    _logger.LogInformation(
                        DataUtilities.GetServiceLogEventId(),
                        string.Format("smptResponse: {0}", smtpResponse));

                    smtp.Disconnect(true);

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    DataUtilities.GetServiceErrorEventId(),
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message));
                
                return false;
            }
        }
    }
}
