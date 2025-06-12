using AccountLib.Contracts;
using AccountLib.Contracts.Users.Response;
using AccountLib.Data;
using AccountLib.Errors;
using AccountLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountLib.Services.UserProfileService
{
	public class UserProfileService(UserManager<ApplicationUser> userManager, ApplicationDbContext db) : IUserProfileService
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
		private async Task<List<string>> getUserRolesAsync(string userId)
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



		public async Task<ResultWithMessage<UserProfileResponse>> GetProfileAsync(string userId)
		{
			var userProfileResponse = new UserProfileResponse();

			var user = await GetByIdAsync(userId);

			if (user is null)
				return new ResultWithMessage<UserProfileResponse>(userProfileResponse, UserErrors.InvalidUserId);

			var roles = await getUserRolesAsync(userId);
			var tenants = await GetUserTenantsAsync(userId);

			userProfileResponse.UserId = user.Id.ToString();
			userProfileResponse.UserName = user.UserName!;
			userProfileResponse.Email = user.Email!;
			userProfileResponse.Roles = roles;
			userProfileResponse.Tenants = tenants;

			return new ResultWithMessage<UserProfileResponse>(userProfileResponse, string.Empty);
		}

		public async Task<ResultWithMessage<ICollection<string>>> GetUserRolesAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user is null)
				return new ResultWithMessage<ICollection<string>>([], UserErrors.InvalidUserId);

			var roles = await _userManager.GetRolesAsync(user);

			return new ResultWithMessage<ICollection<string>>(roles, string.Empty);
		}
	}
}
