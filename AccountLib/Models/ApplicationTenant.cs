using System.ComponentModel.DataAnnotations;

namespace AccountLib.Models
{
	public class ApplicationTenant
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public string? Name { get; set; }
		public string? Description { get; set; }


		public virtual ICollection<ApplicationUserTenantRole> UserTenantRoles { get; set; } = [];
	}
}