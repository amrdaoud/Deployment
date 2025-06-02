using Microsoft.AspNetCore.Identity;

namespace AccountLib.Models
{
	public class ApplicationUserTenantRole : IdentityUserRole<Guid>
	{
		public Guid TenantId { get; set; }

		public virtual ApplicationUser User { get; set; } = default!;
		public virtual ApplicationRole Role { get; set; } = default!;
		public virtual ApplicationTenant Tenant { get; set; } = default!;
	}
}