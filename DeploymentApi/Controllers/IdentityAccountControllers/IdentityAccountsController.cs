using AccountLib.Contracts.IdentityAccount.Request;
using AccountLib.Services.IdentityAccountService;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentApi.Controllers.IdentityAccountControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IdentityAccountsController(IIdentityAccountService IdentityAccountService) : BaseController
	{
		private readonly IIdentityAccountService _IdentityAccountService = IdentityAccountService;


		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterRequest request) => HandleResult(await _IdentityAccountService.RegisterAsync(request));


		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request) => HandleResult(await _IdentityAccountService.LoginAsync(request));


		[HttpPost("forgotPassword")]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
		{
			var sendResetPasswordEmailRequest = new SendResetPasswordEmailRequest
			{
				Email = request.Email,
				BaseUrl = "https://localhost:5001/api/IdentityAccounts/reset-password-form" // test URL
			};

			var result = await _IdentityAccountService.SendResetPasswordEmailAsync(sendResetPasswordEmailRequest);
			return HandleResult(result);
		}


		[HttpPost("resetPassword")]
		public async Task<IActionResult> ResetPassword(ResetPasswordRequest request) => HandleResult(await _IdentityAccountService.ResetPasswordAsync(request));


		// for testing only
		[HttpGet("reset-password-form")]
		public IActionResult ResetPasswordForm(string email, string token)
		{
			return Ok(new
			{
				Message = "Token received",
				Email = email,
				Token = token
			});
		}


		[HttpPost("changePassword")]
		public async Task<IActionResult> ChangePassword(ChangePasswordRequest request) => HandleResult(await _IdentityAccountService.ChangePasswordAsync(request));


		[HttpPost("refreshToken")]
		public async Task<IActionResult> RefreshToken(string refreshToken) => HandleResult(await _IdentityAccountService.RefreshTokenAsync(refreshToken));



		[HttpPost("logout")]
		public async Task<IActionResult> Logout(string refreshToken) => HandleResult(await _IdentityAccountService.LogoutAsync(refreshToken));
	}
}