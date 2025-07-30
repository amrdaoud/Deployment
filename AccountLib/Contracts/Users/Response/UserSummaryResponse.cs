namespace AccountLib.Contracts.Users.Response
{
	public class UserSummaryResponse
	{
		public string Id { get; set; } = default!;
		public string UserName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public bool IsActive { get; set; }
		public List<string> Roles { get; set; } = [];
		public List<string> Tenants { get; set; } = [];
	}
}
