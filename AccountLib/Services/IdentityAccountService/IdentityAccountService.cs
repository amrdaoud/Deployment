

using AccountLib.Contracts;
using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Data;
using AccountLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace AccountLib.Services.IdentityAccountService
{
	public class IdentityAccountService(
		ApplicationDbContext db,
		UserManager<ApplicationUser> userManager,
		RoleManager<ApplicationRole> roleManager,
		IConfiguration configuration) : IIdentityAccountService
	{

		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly IConfiguration _config = configuration;
		private readonly ApplicationDbContext _db = db;
		private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

		public async Task<ResultWithMessage> RegisterAsync(RegisterRequest request)
		{
			var user = new ApplicationUser
			{
				UserName = request.UserName,
				Email = request.Email,
				EmailConfirmed = true,
				NormalizedEmail = request.Email.ToUpper(),
				NormalizedUserName = request.UserName.ToUpper(),
				PhoneNumber = request.PhoneNumber
			};

			var result = await _userManager.CreateAsync(user, request.Password);

			if (!result.Succeeded)
				return new ResultWithMessage(null, result.Errors.First().Description);

			foreach (var tenantRole in request.TenantRoles)
			{
				foreach (var roleId in tenantRole.RoleIds)
				{
					var role = await _roleManager.FindByIdAsync(roleId);

					if (role == null)
						continue;

					_db.UserRoles.Add(new ApplicationUserTenantRole
					{
						UserId = user.Id,
						RoleId = Guid.Parse(roleId),
						TenantId = Guid.Parse(tenantRole.TenantId),
					});
				}
			}

			await _db.SaveChangesAsync();

			return new ResultWithMessage(null, string.Empty);
		}
	}
}
