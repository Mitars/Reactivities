using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Reactivities.Application.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Reactivities.Infrastructure.Email {
    public class EmailSender : IEmailSender {
        private readonly IOptions<SendGridSettings> settings;
        public EmailSender (IOptions<SendGridSettings> settings) {
            this.settings = settings;
        }

        public async Task SendEmailAsync(string userEmail, string emailSubject, string messageText) {
            var client = new SendGridClient(this.settings.Value.Key);
            var message = new SendGridMessage
            {
                From = new EmailAddress("mitarkun@gmail.com", this.settings.Value.User),
                Subject = emailSubject,
                PlainTextContent = messageText,
                HtmlContent = messageText
            };

            message.AddTo(new EmailAddress(userEmail));
            message.SetClickTracking(false, false);

            await client.SendEmailAsync(message);
        }
    }
}