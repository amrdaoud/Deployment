using AccountLib.Configuration;
using Microsoft.Extensions.Configuration;

namespace AccountLib.Contracts
{
	public class AccountIdentityParams
	{
		public ConfigurationManager ConfigurationManager { get; set; } = default!;
		public string ConnectionString { get; set; } = string.Empty;
		public JwtSettings JwtSettings { get; set; } = default!;
	}
}
