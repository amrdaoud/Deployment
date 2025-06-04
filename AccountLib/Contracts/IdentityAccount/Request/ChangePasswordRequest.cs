namespace AccountLib.Contracts.IdentityAccount.Request
{
	public class ChangePasswordRequest
	{
		public string Email { get; set; } = string.Empty;
		public string CurrentPassword { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
	}
}