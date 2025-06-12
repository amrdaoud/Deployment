using AccountLib.Contracts;
using AccountLib.Contracts.Users.Response;
using AccountLib.Data;
using AccountLib.Errors;
using AccountLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountLib.Services.IdentityAccountService.UsersService
{
	public class UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext db) : IUserService
	{

		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly ApplicationDbContext _db = db;


		private async Task<ApplicationUser?> GetByIdAsync(string userId)
		{
			return await _userManager.Users
				.Include(u => u.UserTenantRoles)
				.ThenInclude(utr => utr.Tenant)
				.FirstOrDefaultAsync(u => u.Id.ToString().Trim().ToLower() == userId.Trim().ToLower());
		}
		private async Task<List<string>> GetUserRolesAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			return user == null ? [] : [.. await _userManager.GetRolesAsync(user)];
		}
		private async Task<List<string>> GetUserTenantsAsync(string userId)
		{
			return await _db.UserTenantRoles
				.Include(utr => utr.Tenant)
				.Where(utr => utr.UserId.ToString().Trim().ToLower() == userId.Trim().ToLower() && utr.Tenant != null)
				.Select(utr => utr.Tenant!.Name)
				.Distinct()
				.ToListAsync();
		}



		public async Task<ResultWithMessage> GetProfileAsync(string userId)
		{
			var user = await GetByIdAsync(userId);

			if (user is null)
				return new ResultWithMessage(null, UserErrors.InvalidUserId);

			var roles = await GetUserRolesAsync(userId);
			var tenants = await GetUserTenantsAsync(userId);

			var result = new UserProfileResponse
			{
				UserId = user.Id.ToString(),
				UserName = user.UserName!,
				Email = user.Email!,
				Roles = roles,
				Tenants = tenants
			};

			return new ResultWithMessage(result, string.Empty);
		}
	}
}
