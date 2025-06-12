using AccountLib.Contracts;
using AccountLib.Contracts.Users.Request;
using AccountLib.Contracts.Users.Response;

namespace AccountLib.Services.UserProfileService
{
	public interface IUserProfileService
	{
		Task<ResultWithMessage<UserProfileResponse>> GetProfileAsync(string userId);
		Task<ResultWithMessage<ICollection<string>>> GetUserRolesAsync(string userId);
		Task<ResultWithMessage<bool>> UpdateProfileAsync(string userId, UpdateUserProfileRequest request);
	}
}
