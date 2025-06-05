using AccountLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountLib.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<
																						ApplicationUser,
																						ApplicationRole,
																						Guid,
																						IdentityUserClaim<Guid>,
																						ApplicationUserTenantRole, // Replace IdentityUserRole
																						IdentityUserLogin<Guid>,
																						IdentityRoleClaim<Guid>,
																						IdentityUserToken<Guid>>(options)
	{
		public DbSet<ApplicationTenant> Tenants { get; set; }
		public DbSet<ApplicationUserTenantRole> UserTenantRoles { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }


		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Identity tables
			builder.Entity<ApplicationUser>().ToTable("Users")
				.Property(u => u.Id)
				.HasDefaultValueSql("NEWSEQUENTIALID()");

			builder.Entity<ApplicationRole>().ToTable("Roles")
				.Property(r => r.Id)
				.HasDefaultValueSql("NEWSEQUENTIALID()");

			builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
			builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
			builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
			builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

			// Tenants table
			builder.Entity<ApplicationTenant>().ToTable("Tenants")
				.Property(t => t.Id)
				.HasDefaultValueSql("NEWSEQUENTIALID()");

			// UserTenantRoles table
			builder.Entity<ApplicationUserTenantRole>(entity =>
			{
				entity.ToTable("UserTenantRoles");

				entity.HasKey(x => new { x.UserId, x.RoleId, x.TenantId });

				entity.HasOne(x => x.User)
					  .WithMany(u => u.UserTenantRoles)
					  .HasForeignKey(x => x.UserId);

				entity.HasOne(x => x.Role)
					  .WithMany(r => r.UserTenantRoles)
					  .HasForeignKey(x => x.RoleId);

				entity.HasOne(x => x.Tenant)
					  .WithMany(t => t.UserTenantRoles)
					  .HasForeignKey(x => x.TenantId);
			});

			// Refresh Token
			builder.Entity<RefreshToken>().ToTable("RefreshTokens")
				.Property(u => u.Id)
				.HasDefaultValueSql("NEWSEQUENTIALID()");
		}
	}
}