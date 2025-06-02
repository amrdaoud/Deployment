using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Contracts;



namespace AccountLib.Services.IdentityAccountService
{
	public interface IIdentityAccountService
	{
		Task<ResultWithMessage> RegisterAsync(RegisterRequest request);
		//Task<ResultWithMessage> LoginAsync(LoginRequest request);



		//Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordRequest request);
		//Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordRequest request);
		//Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request);
	}
}
