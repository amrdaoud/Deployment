using Microsoft.AspNetCore.Identity;

namespace AccountLib.Models
{
	public class ApplicationUser : IdentityUser<Guid>
	{





		public virtual ICollection<ApplicationUserTenantRole> UserTenantRoles { get; set; } = [];
		public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
	}
}