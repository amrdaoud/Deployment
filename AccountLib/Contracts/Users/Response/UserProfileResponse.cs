namespace AccountLib.Contracts.Users.Response
{
	public class UserProfileResponse
	{
		public string UserId { get; set; } = default!;
		public string UserName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public List<string> Roles { get; set; } = [];
		public List<string> Tenants { get; set; } = [];
	}
}
