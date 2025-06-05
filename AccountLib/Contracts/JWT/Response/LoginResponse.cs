namespace AccountLib.Contracts.JWT.Response
{
	public class LoginResponse
	{
		public string UserName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public bool IsAuthenticated { get; set; }
		public List<string> Roles { get; set; } = [];
		public List<string> Tenants { get; set; } = [];


		// Token
		public string Token { get; set; } = string.Empty;
		public DateTime TokenExpiry { get; set; }


		// Refresh Token
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime? RefreshTokenExpiry { get; set; }
	}
}
