using AccountLib.Contracts;
using AccountLib.Contracts.Users.Response;
using AccountLib.Data;
using AccountLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountLib.Services.UserManagementService
{
	public class UserManagementService(UserManager<ApplicationUser> userManager, ApplicationDbContext db) : IUserManagementService
	{
		private readonly UserManager<ApplicationUser> _userManager = userManager;
		private readonly ApplicationDbContext _db = db;


		public async Task<ResultWithMessage<List<UserSummaryResponse>>> GetAllUsersAsync()
		{
			var users = _userManager.Users
				.Include(u => u.UserTenantRoles)
				.ThenInclude(utr => utr.Tenant);

			var userList = new List<UserSummaryResponse>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);

				var tenants = user.UserTenantRoles
								  .Select(x => x.Tenant?.Name)
								  .Where(n => n != null)
								  .Distinct()
								  .ToList()!;

				userList.Add(new UserSummaryResponse
				{
					Id = user.Id.ToString(),
					UserName = user.UserName!,
					Email = user.Email!,
					IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTime.UtcNow,
					Roles = [.. roles],
					Tenants = tenants!
				});
			}

			return new ResultWithMessage<List<UserSummaryResponse>>(userList, string.Empty);
		}
	}
}