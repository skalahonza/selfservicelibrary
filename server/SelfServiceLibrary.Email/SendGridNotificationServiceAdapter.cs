using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using SelfServiceLibrary.BL.DTO.User;
using SelfServiceLibrary.BL.Interfaces;

using SendGrid;
using SendGrid.Helpers.Mail;

namespace SelfServiceLibrary.Email
{
    public class SendGridNotificationServiceAdapter : NotificationServiceBase
    {
        private readonly ISendGridClient _client;
        private readonly ILogger<SendGridNotificationServiceAdapter> _log;

        public SendGridNotificationServiceAdapter(
            IUserService userService,
            IMapper mapper,
            ISendGridClient client,
            ILogger<SendGridNotificationServiceAdapter> log)
            :base(userService, mapper)
        {
            _client = client;
            _log = log;
        }

        protected override async Task Send(string title, string message, IEnumerable<UserListDTO> recipients)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress("test@example.com", "Self Service Library"),
                Subject = title,
                HtmlContent = message
            };

            var emails = recipients
                .Where(x => !string.IsNullOrEmpty(x.InfoEmail))
                .Select(x => new EmailAddress(x.InfoEmail, x.InfoFullName))
                .ToList();

            if (emails.Any())
            {

                msg.AddTo(emails[0]);

                if (emails.Count > 1)
                    msg.AddBccs(emails.Skip(1).ToList());

                // Disable click tracking.
                // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
                msg.SetClickTracking(false, false);

                _log.LogInformation("Sending email to {emails} with subject {subject}.", string.Join(",", emails.Select(x => x.Email)), title);
                try
                {
                    var response = await _client.SendEmailAsync(msg);
                    _log.LogInformation("SendGrid response - {StatusCode} - {Response}", response.StatusCode, await response.Body.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    _log.LogCritical(ex, "Error sending an email through SendGrid.");
                }
            }
        }
    }
}
