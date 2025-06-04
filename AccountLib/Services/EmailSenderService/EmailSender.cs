
using AccountLib.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace AccountLib.Services.EmailSenderService
{
	public class EmailSender(IOptions<EmailSettings> emailSettings) : IEmailSender
	{
		private readonly EmailSettings _settings = emailSettings.Value;

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
			{
				Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass),
				EnableSsl = _settings.EnableSsl
			};

			var message = new MailMessage
			{
				From = new MailAddress(_settings.FromEmail, _settings.FromName),
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			message.To.Add(toEmail);
			await client.SendMailAsync(message);
		}
	}
}