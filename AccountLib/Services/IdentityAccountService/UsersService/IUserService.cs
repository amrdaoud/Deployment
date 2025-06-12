using AccountLib.Contracts;

namespace AccountLib.Services.IdentityAccountService.UsersService
{
	public interface IUserService
	{
		Task<ResultWithMessage> GetProfileAsync(string userId);


		//Task<List<string>> GetUserRolesAsync(string userId);
		//Task<List<string>> GetUserTenantsAsync(string userId);
		//Task<bool> UserExistsAsync(string userId);
		//Task<ApplicationUser?> GetByIdAsync(string userId);
		//Task<ResultWithMessage> UpdateProfileAsync(UpdateProfileRequest request);
	}
}
