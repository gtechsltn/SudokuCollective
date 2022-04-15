using System;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Messages;
using SudokuCollective.Logs;
using SudokuCollective.Logs.Utilities;

namespace SudokuCollective.Data.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailMetaData _emailMetaData;
        private readonly IRequestService _requestService;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IEmailMetaData emailMetaData, 
            IRequestService requestService,
            ILogger<EmailService> logger)
        {
            _emailMetaData = emailMetaData;
            _requestService= requestService;
            _logger = logger;
        }
        
        public bool Send(string to, string subject, string html)
        {
            if (string.IsNullOrEmpty(to)) throw new ArgumentNullException(nameof(to));

            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));

            if (string.IsNullOrEmpty(html)) throw new ArgumentNullException(nameof(html));

            // create message
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_emailMetaData.FromEmail));

            email.To.Add(MailboxAddress.Parse(to));

            email.Subject = subject;

            email.Body = new TextPart(TextFormat.Html) { Text = html };

            try
            {
                // send email
                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(_emailMetaData.SmtpServer, _emailMetaData.Port, SecureSocketOptions.Auto);

                    smtp.Authenticate(_emailMetaData.UserName, _emailMetaData.Password);

                    var smtpResponse = smtp.Send(email);

                    SudokuCollectiveLogger.LogInformation<EmailService>(
                        _logger,
                        LogsUtilities.GetSMTPEventId(),
                        string.Format("smptResponse: {0}", smtpResponse),
                        (SudokuCollective.Logs.Models.Request)_requestService.Get());

                    smtp.Disconnect(true);

                    return true;
                }
            }
            catch (Exception e)
            {
                SudokuCollectiveLogger.LogError<EmailService>(
                    _logger,
                    LogsUtilities.GetSMTPEventId(),
                    string.Format(LoggerMessages.ErrorThrownMessage, e.Message),
                    e,
                    (SudokuCollective.Logs.Models.Request)_requestService.Get());
                
                return false;
            }
        }
    }
}
