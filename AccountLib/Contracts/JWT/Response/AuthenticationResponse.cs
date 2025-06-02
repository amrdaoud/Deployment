namespace AccountLib.Contracts.JWT.Response
{
	public class AuthenticationResponse
	{
		public bool IsAuthenticated { get; set; }
		public string? UserName { get; set; }
		public string? Token { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? TokenExpiry { get; set; }
		public DateTime? RefreshTokenExpiry { get; set; }
	}
}
