namespace AccountLib.Services.EmailSenderService
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string toEmail, string subject, string body);
	}
}