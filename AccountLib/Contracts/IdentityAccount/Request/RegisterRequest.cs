namespace AccountLib.Contracts.IdentityAccount.Request
{
	public class RegisterRequest
	{
		public string UserName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; } = string.Empty;
		public List<TenantRoleAssignment> TenantRoles { get; set; } = new();
	}
	public class TenantRoleAssignment
	{
		public string TenantId { get; set; } = string.Empty;
		public List<string> RoleIds { get; set; } = [];
	}
}
