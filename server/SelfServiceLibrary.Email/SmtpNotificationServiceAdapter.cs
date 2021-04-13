using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;
using SelfServiceLibrary.Email.Options;

namespace SelfServiceLibrary.Email
{
    public class SmtpNotificationServiceAdapter : NotificationServiceBase
    {
        private readonly IOptions<SmtpNotificationServiceOptions> _options;
        private readonly ILogger<SmtpNotificationServiceAdapter> _log;

        public SmtpNotificationServiceAdapter(
            IUserService userService,
            IMapper mapper,
            IOptions<SmtpNotificationServiceOptions> options,
            ILogger<SmtpNotificationServiceAdapter> log)
            : base(userService, mapper)
        {
            _options = options;
            _log = log;
        }

        protected override async Task Send(string title, string message, IEnumerable<UserListDTO> recipients)
        {
            using var client = new SmtpClient(_options.Value.RelayAddress, 25);

            using var emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(_options.Value.From, _options.Value.FromName);
            emailMessage.Subject = title;
            emailMessage.IsBodyHtml = true;
            emailMessage.Body = message;

            var emails = recipients
               .Where(x => !string.IsNullOrEmpty(x.InfoEmail))
               .Select(x => new MailAddress(x.InfoEmail, x.InfoFullName))
               .ToList();

            if (emails.Any())
            {
                emailMessage.To.Add(emails[0]);

                if (emails.Count > 1)
                {
                    foreach (var item in emails.Skip(1))
                        emailMessage.Bcc.Add(item);
                }
            }

            try
            {
                await client.SendMailAsync(emailMessage);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, "Error sending an email through SMTP.");
            }            
        }
    }
}
