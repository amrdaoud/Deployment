using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Contracts;
using AccountLib.Contracts.JWT.Response;



namespace AccountLib.Services.IdentityAccountService
{
	public interface IIdentityAccountService
	{
		Task<ResultWithMessage<bool>> RegisterAsync(RegisterRequest request);
		Task<ResultWithMessage<LoginResponse>> LoginAsync(LoginRequest request);
		Task<ResultWithMessage<bool>> SendResetPasswordEmailAsync(SendResetPasswordEmailRequest request);
		Task<ResultWithMessage<bool>> ResetPasswordAsync(ResetPasswordRequest request);
		Task<ResultWithMessage<bool>> ChangePasswordAsync(ChangePasswordRequest request);
		Task<ResultWithMessage<LoginResponse>> RefreshTokenAsync(string refreshToken);
		Task<ResultWithMessage<bool>> LogoutAsync(string refreshToken);
	}
}