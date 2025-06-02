using Microsoft.AspNetCore.Identity;

namespace AccountLib.Models
{
	public class ApplicationRole : IdentityRole<Guid>
	{
		public virtual ICollection<ApplicationUserTenantRole> UserTenantRoles { get; set; } = [];
	}
}