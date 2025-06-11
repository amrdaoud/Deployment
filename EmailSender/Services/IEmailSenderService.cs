namespace EmailSender.Services
{
	public interface IEmailSenderService
	{
		Task SendEmailAsync(string toEmail, string subject, string body);
	}
}