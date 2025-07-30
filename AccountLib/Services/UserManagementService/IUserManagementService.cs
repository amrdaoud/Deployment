using AccountLib.Contracts;
using AccountLib.Contracts.Users.Response;

namespace AccountLib.Services.UserManagementService
{
	public interface IUserManagementService
	{
		Task<ResultWithMessage<List<UserSummaryResponse>>> GetAllUsersAsync();
	}
}