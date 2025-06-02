namespace AccountLib.Contracts.IdentityAccount.Request
{
	public class LoginRequest
	{
		public string EmailOrUsername { get; set; } = default!;
		public string Password { get; set; } = default!;
	}
}
