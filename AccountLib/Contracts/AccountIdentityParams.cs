using AccountLib.Configuration;

namespace AccountLib.Contracts
{
	public class AccountIdentityParams
	{
		public JwtSettings JwtSettings { get; set; } = default!;
	}
}
