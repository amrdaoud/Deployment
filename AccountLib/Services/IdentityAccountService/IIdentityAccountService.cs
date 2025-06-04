using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Contracts;
using Microsoft.AspNetCore.Identity;



namespace AccountLib.Services.IdentityAccountService
{
	public interface IIdentityAccountService
	{
		Task<ResultWithMessage> RegisterAsync(RegisterRequest request);
		Task<ResultWithMessage> LoginAsync(LoginRequest request);
		Task<ResultWithMessage> SendResetPasswordEmailAsync(SendResetPasswordEmailRequest request);
		Task<ResultWithMessage> ResetPasswordAsync(ResetPasswordRequest request);


		//Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request);
	}
}