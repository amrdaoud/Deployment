namespace AccountLib.Contracts.IdentityAccount.Request
{
	public class SendResetPasswordEmailRequest
	{
		public string Email { get; set; } = string.Empty;
		public string BaseUrl { get; set; } = string.Empty;
	}
}
