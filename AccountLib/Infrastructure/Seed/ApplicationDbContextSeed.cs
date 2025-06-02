using AccountLib.Data;
using AccountLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccountLib.Infrastructure.Seed
{
	public static class ApplicationDbContextSeed
	{
		public static async Task SeedAsync(IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

			// Ensure database is created
			await context.Database.MigrateAsync();

			// 1. Seed Tenant
			var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Name == "Tenant1");
			if (tenant == null)
			{
				tenant = new ApplicationTenant
				{
					Id = Guid.NewGuid(),
					Name = "Tenant1"
				};
				context.Tenants.Add(tenant);
				await context.SaveChangesAsync();
			}

			// 2. Seed Roles
			string[] roleNames = { "SuperUser", "Admin", "Developer" };

			foreach (var roleName in roleNames)
			{
				var role = await roleManager.FindByNameAsync(roleName);

				if (role == null)
				{
					role = new ApplicationRole
					{
						Id = Guid.NewGuid(),
						Name = roleName,
						NormalizedName = roleName.ToUpper(),
					};
					await roleManager.CreateAsync(role);
				}
			}

			// 3. Seed User
			var user = await userManager.FindByEmailAsync("Saleem.Kassab@Syriatel.net");

			if (user == null)
			{
				user = new ApplicationUser
				{
					Id = Guid.NewGuid(),
					Email = "Saleem.Kassab@Syriatel.net",
					NormalizedEmail = "SALEEM.KASSAB@SYRIATEL.NET",
					UserName = "SaleemK",
					NormalizedUserName = "SALEEMK",
					PhoneNumber = "993998187",
					EmailConfirmed = true,
					PhoneNumberConfirmed = true
				};

				var result = await userManager.CreateAsync(user, "P@ssw0rd123@");

				if (!result.Succeeded)
					throw new Exception("Failed to create Saleem user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
			}

			// 4. Assign Role to User in Tenant (UserTenantRoles)
			var superUserRole = await roleManager.FindByNameAsync("SuperUser");

			var existingLink = await context.UserTenantRoles.FirstOrDefaultAsync(r =>
				r.UserId == user.Id &&
				r.TenantId == tenant.Id &&
				r.RoleId == superUserRole.Id);

			if (existingLink == null)
			{
				var userTenantRole = new ApplicationUserTenantRole
				{
					UserId = user.Id,
					TenantId = tenant.Id,
					RoleId = superUserRole.Id
				};

				context.UserTenantRoles.Add(userTenantRole);
				await context.SaveChangesAsync();
			}
		}
	}
}