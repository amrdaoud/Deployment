using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Contracts;



namespace AccountLib.Services.IdentityAccountService
{
	public interface IIdentityAccountService
	{
		Task<ResultWithMessage> RegisterAsync(RegisterRequest request);
		Task<ResultWithMessage> LoginAsync(LoginRequest request);
		Task<ResultWithMessage> SendResetPasswordEmailAsync(SendResetPasswordEmailRequest request);
		Task<ResultWithMessage> ResetPasswordAsync(ResetPasswordRequest request);
		Task<ResultWithMessage> ChangePasswordAsync(ChangePasswordRequest request);
		Task<ResultWithMessage> RefreshTokenAsync(string refreshToken);
	}
}